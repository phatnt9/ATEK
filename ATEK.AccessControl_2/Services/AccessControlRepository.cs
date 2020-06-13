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
using System.Collections.ObjectModel;
using Microsoft.Office.Core;
using System.CodeDom;

namespace ATEK.AccessControl_2.Services
{
    public class AccessControlRepository : IAccessControlRepository
    {
        private FirestoreDb db;
        private AccessControlContext _context;
        private string _firebaseProfilesCollection;
        private string _firebaseClassesCollection;
        private string _firebaseGatesCollection;

        private string path = AppDomain.CurrentDomain.BaseDirectory + @"cloudfire.json";
        //private string path = AppDomain.CurrentDomain.BaseDirectory + @"cloudfire_nguyen.json";

        public AccessControlRepository()
        {
            _context = new AccessControlContext();
            Firebase_SetCredential();
            Firebase_AuthExplicit("phatdemolockmode", path);
        }

        #region Firebase

        public async void Firebase_SetTime()
        {
            DateTime data = DateTime.Now;
            Dictionary<string, object> date = new Dictionary<string, object>
            {
                { "timeNow", DateTime.SpecifyKind(data, DateTimeKind.Utc)}
            };
            DocumentReference classRef = await db.Collection("TimeTest").AddAsync(date);
        }

        public async void Firebase_GetTime()
        {
            Query allCitiesQuery = db.Collection("TimeTest");
            QuerySnapshot allCitiesQuerySnapshot = await allCitiesQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Console.WriteLine("Document data for {0} document:", documentSnapshot.Id);
                Dictionary<string, object> city = documentSnapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in city)
                {
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                }
                Console.WriteLine("");
            }
        }

        public bool Firebase_AddClass(Class @class)
        {
            try
            {
                DocumentReference classRef = db.Collection(_firebaseClassesCollection).Document(@class.Id.ToString());
                var writeResult = classRef.SetAsync(@class);
                writeResult.Wait(2000);
                switch (writeResult.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            return true;
                        }
                    case TaskStatus.Faulted:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public bool Firebase_UpdateClass(Class @class)
        {
            try
            {
                DocumentReference classRef = db.Collection(_firebaseClassesCollection).Document(@class.Id.ToString());
                var writeResult = classRef.UpdateAsync("Name", @class.Name);
                writeResult.Wait(2000);
                switch (writeResult.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            return true;
                        }
                    case TaskStatus.Faulted:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public bool Firebase_RemoveClass(Class @class)
        {
            try
            {
                DocumentReference classRef = db.Collection(_firebaseClassesCollection).Document(@class.Id.ToString());
                var writeResult = classRef.DeleteAsync();
                writeResult.Wait(2000);
                switch (writeResult.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            return true;
                        }
                    case TaskStatus.Faulted:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public bool Firebase_AddProfileGate(Profile profile, Gate gate)
        {
            try
            {
                DocumentReference gateRef = db.Collection(_firebaseGatesCollection).Document(gate.FirebaseId);
                DocumentReference profileGatesRef = gateRef.Collection("Profiles").Document(profile.Pinno);
                var writeResult = profileGatesRef.SetAsync(profile);
                writeResult.Wait(2000);
                switch (writeResult.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            return true;
                        }
                    case TaskStatus.Faulted:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public bool Firebase_RemoveProfileGate(Profile profile, Gate gate)
        {
            try
            {
                DocumentReference gateRef = db.Collection(_firebaseGatesCollection).Document(gate.FirebaseId);
                DocumentReference profileGatesRef = gateRef.Collection("Profiles").Document(profile.Pinno);
                var writeResult = profileGatesRef.DeleteAsync();
                writeResult.Wait(2000);
                switch (writeResult.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            return true;
                        }
                    case TaskStatus.Faulted:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public async Task<bool> Firebase_AddClassAsync(Class @class)
        {
            try
            {
                DocumentReference classRef = db.Collection(_firebaseClassesCollection).Document(@class.Id.ToString());
                var writeResult = classRef.SetAsync(@class);
                await writeResult;
                switch (writeResult.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            return true;
                        }
                    case TaskStatus.Faulted:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public async Task<bool> Firebase_AddProfileGateAsync(Profile profile, Gate gate)
        {
            try
            {
                DocumentReference gateRef = db.Collection(_firebaseGatesCollection).Document(gate.FirebaseId);
                DocumentReference profileGatesRef = gateRef.Collection("Profiles").Document(profile.Pinno);
                var writeResult = profileGatesRef.SetAsync(profile);
                await writeResult;
                switch (writeResult.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            return true;
                        }
                    case TaskStatus.Faulted:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public async Task<bool> Firebase_RemoveProfileGateAsync(Profile profile, Gate gate)
        {
            try
            {
                DocumentReference gateRef = db.Collection(_firebaseGatesCollection).Document(gate.FirebaseId);
                DocumentReference profileGatesRef = gateRef.Collection("Profiles").Document(profile.Pinno);
                var writeResult = profileGatesRef.DeleteAsync();
                await writeResult;
                switch (writeResult.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            return true;
                        }
                    case TaskStatus.Faulted:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public async Task<IEnumerable<Gate>> Firebase_GetGatesAsync()
        {
            try
            {
                CollectionReference gatesRef = db.Collection(_firebaseGatesCollection);
                gatesRef.Listen(snapShots => OnFirebaseGatesChange(snapShots));
                var querySnapshot = gatesRef.GetSnapshotAsync();
                await querySnapshot;
                switch (querySnapshot.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            List<Gate> gates = new List<Gate>();
                            List<DocumentSnapshot> documents = querySnapshot.Result.ToList();
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
                    case TaskStatus.Faulted:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }

        public void OnFirebaseGatesChange(QuerySnapshot snapShots)
        {
            Console.WriteLine("Callback received document snapshot.");
            foreach (DocumentChange change in snapShots.Changes)
            {
                if (change.ChangeType.ToString() == "Added")
                {
                    Console.WriteLine("New gate: {0}", change.Document.Id);
                    //Console.WriteLine("Document exists? {0}", change.Document.Exists);
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
        }

        private void Firebase_SetCredential()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("phatdemolockmode");
            //db = FirestoreDb.Create("testaccesscontrol-4ccde");
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
            _firebaseClassesCollection = $"{Properties.Settings.Default.serviceAccountName}_classes";
            _firebaseGatesCollection = $"{Properties.Settings.Default.serviceAccountName}_gates";
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
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
                //Console.WriteLine(_context.Entry(profile).State.ToString());
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
            Console.WriteLine(exception.GetType().ToString());
            switch (exception)
            {
                case Grpc.Core.RpcException ex:
                    {
                        switch (ex.StatusCode)
                        {
                            /*
                             * Stream removed: Lan dau tien ko connect duoc firebase khi bi rut mang
                             * Failed to connect to all addresses: Cac lan sau do deu bao ko connect duoc firebase
                             */
                            case Grpc.Core.StatusCode.Unknown:
                                {
                                    Console.WriteLine(ex.Status.Detail);
                                    return true;
                                }
                            case Grpc.Core.StatusCode.Unavailable:
                                {
                                    Console.WriteLine(ex.Status.Detail);
                                    return true;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case AggregateException ex:
                    {
                        var innerException = ex.InnerException as Grpc.Core.RpcException;
                        switch (innerException.StatusCode)
                        {
                            /*
                             * Stream removed: Lan dau tien ko connect duoc firebase khi bi rut mang
                             * Failed to connect to all addresses: Cac lan sau do deu bao ko connect duoc firebase
                             */
                            case Grpc.Core.StatusCode.Unknown:
                                {
                                    Console.WriteLine(innerException.Status.Detail);
                                    return true;
                                }
                            case Grpc.Core.StatusCode.Unavailable:
                                {
                                    Console.WriteLine(innerException.Status.Detail);
                                    return true;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case DbUpdateException ex:
                    {
                        var innerException = ex.InnerException as SqlException;
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
                        throw new NotImplementedException();
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
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

//Dictionary<string, object> profileData = new Dictionary<string, object>
            //{
            //    { nameof(profile.Id), profile.Id },
            //    { nameof(profile.Pinno), profile.Pinno },
            //    { nameof(profile.Adno), profile.Adno },
            //    { nameof(profile.Name), profile.Name },
            //    { nameof(profile.Gender), profile.Gender },
            //    { nameof(profile.DateOfBirth), DateTime.SpecifyKind(profile.DateOfBirth, DateTimeKind.Utc) },
            //    { nameof(profile.DateOfIssue), DateTime.SpecifyKind(profile.DateOfIssue, DateTimeKind.Utc) },
            //    { nameof(profile.Email), profile.Email },
            //    { nameof(profile.Address), profile.Address },
            //    { nameof(profile.Phone), profile.Phone },
            //    { nameof(profile.Status), profile.Status },
            //    { nameof(profile.Image), profile.Image },
            //    { nameof(profile.DateToLock), DateTime.SpecifyKind(profile.DateToLock, DateTimeKind.Utc) },
            //    { nameof(profile.CheckDateToLock), profile.CheckDateToLock },
            //    { nameof(profile.LicensePlate), profile.LicensePlate },
            //    { nameof(profile.DateCreated),DateTime.SpecifyKind(profile.DateCreated, DateTimeKind.Utc) },
            //    { nameof(profile.DateModified), DateTime.SpecifyKind(profile.DateModified, DateTimeKind.Utc) },
            //    { nameof(profile.Class), profile.Class.Name },
            //    { nameof(profile.ClassId), profile.ClassId }
            //};
            //await profileRef.SetAsync(profileData);
 */