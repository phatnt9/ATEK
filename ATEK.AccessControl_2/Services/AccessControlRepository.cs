using ATEK.Data.Contexts;
using ATEK.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using Google.Cloud.Firestore.V1;
using System.Threading;
using DocumentChange = Google.Cloud.Firestore.DocumentChange;
using System.Windows;

namespace ATEK.AccessControl_2.Services
{
    public class AccessControlRepository : IAccessControlRepository
    {
        private FirestoreDb db;
        private AccessControlContext _context;
        private string _firebaseProfilesCollection;
        private string _firebaseGatesCollection;
        private string path = AppDomain.CurrentDomain.BaseDirectory + @"cloudfire.json";

        public AccessControlRepository()
        {
            _context = new AccessControlContext();
            Firebase_SetCredential();
            Firebase_AuthExplicit("phatdemolockmode", path);
        }

        #region Firebase

        private void Firebase_SetCredential()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("phatdemolockmode");
        }

        private object Firebase_AuthExplicit(string projectId, string jsonPath)
        {
            // Explicitly use service account credentials by specifying
            // the private key file.
            var credential = GoogleCredential.FromFile(jsonPath);
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            string serviceAccountEmail = ((ServiceAccountCredential)credential.UnderlyingCredential).Id;
            string serviceAccountName = serviceAccountEmail.Split('@')[0];
            if (!serviceAccountName.Equals(Properties.Settings.Default.serviceAccountName))
            {
                Properties.Settings.Default.serviceAccountName = serviceAccountName;
                Properties.Settings.Default.Save();
            }
            _firebaseProfilesCollection = $"{Properties.Settings.Default.serviceAccountName}_profiles";
            _firebaseGatesCollection = $"{Properties.Settings.Default.serviceAccountName}_gates";
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }

        public async void Firebase_AddProfileAsync(Profile profile)
        {
            DocumentReference profileRef = db.Collection(_firebaseProfilesCollection).Document(profile.Pinno);
            CollectionReference profileGroupsRef = profileRef.Collection("ProfileGroups");
            CollectionReference profileGatesRef = profileRef.Collection("ProfileGates");
            Dictionary<string, object> profileData = new Dictionary<string, object>
            {
                { nameof(profile.Id), profile.Id },
                { nameof(profile.Pinno), profile.Pinno },
                { nameof(profile.Adno), profile.Adno },
                { nameof(profile.Name), profile.Name },
                //{ nameof(profile.Gender), profile.Gender },
                //{ nameof(profile.DateOfBirth), DateTime.SpecifyKind(profile.DateOfBirth, DateTimeKind.Utc) },
                //{ nameof(profile.DateOfIssue), DateTime.SpecifyKind(profile.DateOfIssue, DateTimeKind.Utc) },
                //{ nameof(profile.Email), profile.Email },
                //{ nameof(profile.Address), profile.Address },
                //{ nameof(profile.Phone), profile.Phone },
                { nameof(profile.Status), profile.Status },
                { nameof(profile.Image), profile.Image },
                { nameof(profile.DateToLock), DateTime.SpecifyKind(profile.DateToLock, DateTimeKind.Utc) },
                { nameof(profile.CheckDateToLock), profile.CheckDateToLock },
                { nameof(profile.LicensePlate), profile.LicensePlate },
                //{ nameof(profile.DateCreated),DateTime.SpecifyKind(profile.DateCreated, DateTimeKind.Utc) },
                //{ nameof(profile.DateModified), DateTime.SpecifyKind(profile.DateModified, DateTimeKind.Utc) },
                { nameof(profile.Class), profile.Class.Name },
                //{ nameof(profile.ClassId), profile.ClassId }
            };
            await profileRef.SetAsync(profileData);

            foreach (var pg in profile.ProfileGroups)
            {
                Dictionary<string, object> profileGroupsData = new Dictionary<string, object>();
                profileGroupsData.Add(nameof(pg.Group.Name), pg.Group.Name);
                await profileGroupsRef.AddAsync(profileGroupsData);
            }
            foreach (var pg in profile.ProfileGates)
            {
                Dictionary<string, object> profileGatesData = new Dictionary<string, object>();
                profileGatesData.Add(nameof(pg.Gate.Name), pg.Gate.Name);
                await profileGatesRef.AddAsync(profileGatesData);
            }
        }

        public IEnumerable<Gate> Firebase_GetGates()
        {
            List<Gate> gates = new List<Gate>();
            CollectionReference gatesRef = db.Collection(_firebaseGatesCollection);
            gatesRef.Listen(snapShots => OnFirebaseGatesChange(snapShots));
            List<DocumentSnapshot> documents = gatesRef.GetSnapshotAsync().Result.ToList();
            foreach (var rawGate in documents)
            {
                if (rawGate.Exists)
                {
                    Gate gate = new Gate();
                    gate.FirebaseId = rawGate.Id;
                    Dictionary<string, object> gateData = rawGate.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in gateData)
                    {
                        switch (pair.Key)
                        {
                            case "Name":
                                {
                                    gate.Name = pair.Value.ToString();
                                    break;
                                }
                            case "Note":
                                {
                                    gate.Note = pair.Value.ToString();
                                    break;
                                }
                            case "Status":
                                {
                                    gate.Status = pair.Value.ToString();
                                    break;
                                }
                        }
                    }
                    gates.Add(gate);
                }
            }
            return gates;
        }

        public List<Profile> Firebase_GetProfiles()
        {
            List<Profile> profiles = new List<Profile>();
            CollectionReference profilesRef = db.Collection(_firebaseProfilesCollection);
            QuerySnapshot snapshot = profilesRef.GetSnapshotAsync().Result;
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                var profile = new Profile();
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                profile.Id = int.Parse(documentDictionary["Id"].ToString());
                profile.Pinno = documentDictionary["Pinno"].ToString();
                profile.Adno = documentDictionary["Adno"].ToString();
                profile.Name = documentDictionary["Name"].ToString();
                profile.Gender = documentDictionary["Gender"].ToString();
                profile.DateOfBirth = DateTime.Parse(documentDictionary["DateOfBirth"].ToString());
                profile.DateOfIssue = DateTime.Parse(documentDictionary["DateOfIssue"].ToString());
                profile.Email = documentDictionary["Email"].ToString();
                profile.Address = documentDictionary["Address"].ToString();
                profile.Phone = documentDictionary["Phone"].ToString();
                profile.Status = documentDictionary["Status"].ToString();
                profile.Image = documentDictionary["Image"].ToString();
                profile.DateToLock = DateTime.Parse(documentDictionary["DateToLock"].ToString());
                profile.CheckDateToLock = bool.Parse(documentDictionary["CheckDateToLock"].ToString());
                profile.LicensePlate = documentDictionary["LicensePlate"].ToString();
                profile.DateCreated = DateTime.Parse(documentDictionary["DateCreated"].ToString());
                profile.DateModified = DateTime.Parse(documentDictionary["DateModified"].ToString());
                //profile.Class = int.Parse(documentDictionary["Id"].ToString());
                profile.ClassId = int.Parse(documentDictionary["ClassId"].ToString());
            }
            return profiles;
        }

        public void OnFirebaseGatesChange(QuerySnapshot snapShots)
        {
            Console.WriteLine("Callback received documentd snapshot.");
            foreach (DocumentChange change in snapShots.Changes)
            {
                if (change.ChangeType.ToString() == "Added")
                {
                    Console.WriteLine("New gate: {0}", change.Document.Id);
                    Console.WriteLine("Document exists? {0}", change.Document.Exists);
                    if (change.Document.Exists)
                    {
                        Console.WriteLine("Document data for {0}:", change.Document.Id);
                        Dictionary<string, object> city = change.Document.ToDictionary();
                        foreach (KeyValuePair<string, object> pair in city)
                        {
                            Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                        }
                    }
                }
                else if (change.ChangeType.ToString() == "Modified")
                {
                    Console.WriteLine("Modified gate: {0}", change.Document.Id);
                    Console.WriteLine("Document exists? {0}", change.Document.Exists);
                    if (change.Document.Exists)
                    {
                        Console.WriteLine("Document data for {0}:", change.Document.Id);
                        Dictionary<string, object> city = change.Document.ToDictionary();
                        foreach (KeyValuePair<string, object> pair in city)
                        {
                            Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                        }
                    }
                }
                else if (change.ChangeType.ToString() == "Removed")
                {
                    Console.WriteLine("Removed gate: {0}", change.Document.Id);
                    Console.WriteLine("Document exists? {0}", change.Document.Exists);
                    if (change.Document.Exists)
                    {
                        Console.WriteLine("Document data for {0}:", change.Document.Id);
                        Dictionary<string, object> city = change.Document.ToDictionary();
                        foreach (KeyValuePair<string, object> pair in city)
                        {
                            Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                        }
                    }
                }
            }
            //Console.WriteLine("Callback received documentd snapshot.");
            //foreach (DocumentSnapshot docGate in snapShots.Documents)
            //{
            //    Console.WriteLine("Document exists? {0}", docGate.Exists);
            //    if (docGate.Exists)
            //    {
            //        Console.WriteLine("Document data for {0} document:", docGate.Id);
            //        Dictionary<string, object> city = docGate.ToDictionary();
            //        foreach (KeyValuePair<string, object> pair in city)
            //        {
            //            Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            //        }
            //    }
            //}
        }

        #endregion Firebase

        #region EntityFramework

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public IEnumerable<Profile> GetProfiles()
        {
            return _context.Profiles.Include(p => p.ProfileGates).Include(p => p.ProfileGroups).ToList();
        }

        public IEnumerable<Class> GetClasses()
        {
            return _context.Classes.Include(c => c.Profiles).ToList();
        }

        public IEnumerable<Gate> GetGates()
        {
            return _context.Gates.Include(g => g.ProfileGates).ToList();
        }

        public IEnumerable<Group> GetGroups()
        {
            return _context.Groups.Include(g => g.ProfileGroups).ToList();
        }

        public bool AddProfile(Profile profile)
        {
            try
            {
                _context.Profiles.Add(profile);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                    _context.Profiles.Remove(profile);
                }
                return false;
            }
        }

        public bool AddClass(Class @class)
        {
            try
            {
                _context.Classes.Add(@class);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                    _context.Classes.Remove(@class);
                }
                return false;
            }
        }

        public bool AddGate(Gate gate)
        {
            try
            {
                _context.Gates.Add(gate);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                    _context.Gates.Remove(gate);
                }
                return false;
            }
        }

        public bool AddGroup(Group group)
        {
            try
            {
                _context.Groups.Add(group);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                    _context.Groups.Remove(group);
                }
                return false;
            }
        }

        public bool AddProfileGroup(Profile profile, Group group)
        {
            var addItem = new ProfileGroup() { ProfileId = profile.Id, GroupId = group.Id };
            try
            {
                _context.Add(addItem);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                    _context.Remove(addItem);
                }
                return false;
            }
        }

        public bool AddProfileGate(Profile profile, Gate gate)
        {
            var addItem = new ProfileGate() { ProfileId = profile.Id, GateId = gate.Id };
            try
            {
                _context.Add(addItem);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                    _context.Remove(addItem);
                }
                return false;
            }
        }

        public bool UpdateProfile(Profile profile)
        {
            try
            {
                if (!_context.Profiles.Local.Any(c => c.Id == profile.Id))
                {
                    _context.Profiles.Attach(profile);
                }
                _context.Entry(profile).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                }
                return false;
            }
        }

        public bool UpdateClass(Class @class)
        {
            try
            {
                if (!_context.Classes.Local.Any(c => c.Id == @class.Id))
                {
                    _context.Classes.Attach(@class);
                }
                _context.Entry(@class).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                }
                return false;
            }
        }

        public bool UpdateGate(Gate gate)
        {
            try
            {
                if (!_context.Gates.Local.Any(c => c.Id == gate.Id))
                {
                    _context.Gates.Attach(gate);
                }
                _context.Entry(gate).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                }
                return false;
            }
        }

        public bool UpdateGroup(Group group)
        {
            try
            {
                if (!_context.Groups.Local.Any(c => c.Id == group.Id))
                {
                    _context.Groups.Attach(group);
                }
                _context.Entry(group).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                }
                return false;
            }
        }

        public bool RemoveProfiles(IEnumerable<Profile> profiles)
        {
            try
            {
                _context.RemoveRange(profiles);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                }
                return false;
            }
        }

        public bool RemoveClasses(IEnumerable<Class> classes)
        {
            try
            {
                _context.RemoveRange(classes);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                }
                return false;
            }
        }

        public bool RemoveGates(IEnumerable<Gate> gates)
        {
            try
            {
                _context.RemoveRange(gates);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                }
                return false;
            }
        }

        public bool RemoveGroups(IEnumerable<Group> groups)
        {
            try
            {
                _context.RemoveRange(groups);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                }
                return false;
            }
        }

        public bool RemoveProfileGroup(Profile profile, Group group)
        {
            //var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == profile.Id);
            //_context.Entry(data).State = EntityState.Deleted;
            //_context.SaveChanges();
            //return true;

            var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == profile.Id);
            if (data != null)
            {
                Console.WriteLine($"Remove Co ne.{profile.Id}");
                _context.Entry(data).State = EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            else
            {
                Console.WriteLine($"Remove Ko co.{profile.Id}");
                var removeItem = new ProfileGroup() { GroupId = group.Id, ProfileId = profile.Id };
                _context.Remove(removeItem);
                _context.SaveChanges();
                return true;
            }
        }

        public bool RemoveProfileGate(Profile profile, Gate gate)
        {
            var data = gate.ProfileGates.FirstOrDefault(g => g.ProfileId == profile.Id);
            if (data != null)
            {
                Console.WriteLine($"Remove Co ne.{profile.Id}");
                _context.Entry(data).State = EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            else
            {
                Console.WriteLine($"Remove Ko co.{profile.Id}");
                var removeItem = new ProfileGate() { GateId = gate.Id, ProfileId = profile.Id };
                _context.Remove(removeItem);
                _context.SaveChanges();
                return true;
            }
        }

        public IEnumerable<Profile> LoadProfilesOfGroup(int groupId)
        {
            var group = _context.Groups.AsQueryable().Where(g => g.Id == groupId)
                .Select(g => new
                {
                    Group = g,
                    Profiles = g.ProfileGroups.Select(pg => pg.Profile)
                }).FirstOrDefault();
            return group.Profiles;
        }

        public IEnumerable<Profile> LoadProfilesOfGate(int gateId)
        {
            var gate = _context.Gates.AsQueryable().Where(g => g.Id == gateId)
              .Select(g => new
              {
                  Gate = g,
                  Profiles = g.ProfileGates.Select(pg => pg.Profile)
              }).FirstOrDefault();
            return gate.Profiles;
        }

        public IEnumerable<Group> LoadGroupsOfProfile(int profileId)
        {
            var profile = _context.Profiles.AsQueryable().Where(p => p.Id == profileId)
             .Select(p => new
             {
                 Profile = p,
                 Groups = p.ProfileGroups.Select(pg => pg.Group)
             }).FirstOrDefault();
            return profile.Groups;
        }

        public IEnumerable<Gate> LoadGatesOfProfile(int profileId)
        {
            var profile = _context.Profiles.AsQueryable().Where(p => p.Id == profileId)
             .Select(p => new
             {
                 Profile = p,
                 Gates = p.ProfileGates.Select(pg => pg.Gate)
             }).FirstOrDefault();
            return profile.Gates;
        }

        public virtual bool HandleException(Exception exception)
        {
            MessageBox.Show("ERROR IN DATABASE CONTEXT");
            SqlException innerException = exception.InnerException as SqlException;
            if (innerException != null)
            {
                switch (innerException.Number)
                {
                    case 2601:
                        {
                            Console.WriteLine(innerException.Message);
                            Console.WriteLine("Duplicated Pinno");
                            return true;
                        }
                    case 2627:
                        {
                            Console.WriteLine(innerException.Message);
                            Console.WriteLine("Duplicated GroupProfiles");
                            return true;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion EntityFramework
    }
}

/*
            //var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == profile.Id);
            //if (data != null)
            //{
            //    Console.WriteLine($"Add Co ne.{profile.Id}");
            //    Console.WriteLine(_context.Entry(data).State.ToString());
            //    _context.Entry(data).State = EntityState.Added;
            //    _context.SaveChanges();
            //    return true;
            //}
            //else
            //{
            //    Console.WriteLine($"Add Ko co.{profile.Id}");
            //    var addItem = new ProfileGroup() { GroupId = group.Id, ProfileId = profile.Id };
            //    _context.Add(addItem);
            //    Console.WriteLine(_context.Entry(addItem).State.ToString());
            //    _context.SaveChanges();
            //    return true;
            //}
 */