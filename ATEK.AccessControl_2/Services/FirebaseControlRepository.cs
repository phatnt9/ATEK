using ATEK.Domain.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Services
{
    public class FirebaseControlRepository : IFirebaseControlRepository
    {
        private string _firebaseProfilesCollection = "bvis_profiles";
        private FirestoreDb db;
        private string path = AppDomain.CurrentDomain.BaseDirectory + @"cloudfire.json";

        public FirebaseControlRepository()
        {
            SetCredential();
            AuthExplicit("phatdemolockmode", path);
            //ReadData();
        }

        public void SetCredential()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("phatdemolockmode");
        }

        public async void AddProfile(Profile profile)
        {
            Google.Cloud.Firestore.DocumentReference docRef = db.Collection(_firebaseProfilesCollection).Document(profile.Pinno);
            Dictionary<string, object> profileData = new Dictionary<string, object>
            {
                { nameof(profile.Id), profile.Id },
                { nameof(profile.Pinno), profile.Pinno },
                { nameof(profile.Adno), profile.Adno },
                { nameof(profile.Name), profile.Name },
                { nameof(profile.Gender), profile.Gender },
                { nameof(profile.DateOfBirth), DateTime.SpecifyKind(profile.DateOfBirth, DateTimeKind.Utc) },
                { nameof(profile.DateOfIssue), DateTime.SpecifyKind(profile.DateOfIssue, DateTimeKind.Utc) },
                { nameof(profile.Email), profile.Email },
                { nameof(profile.Address), profile.Address },
                { nameof(profile.Phone), profile.Phone },
                { nameof(profile.Status), profile.Status },
                { nameof(profile.Image), profile.Image },
                { nameof(profile.DateToLock), DateTime.SpecifyKind(profile.DateToLock, DateTimeKind.Utc) },
                { nameof(profile.CheckDateToLock), profile.CheckDateToLock },
                { nameof(profile.LicensePlate), profile.LicensePlate },
                { nameof(profile.DateCreated),DateTime.SpecifyKind(profile.DateCreated, DateTimeKind.Utc) },
                { nameof(profile.DateModified), DateTime.SpecifyKind(profile.DateModified, DateTimeKind.Utc) },
                { nameof(profile.Class), profile.Class.Name },
                { nameof(profile.ClassId), profile.ClassId }
            };
            //Dictionary<string, object> objectExample = new Dictionary<string, object>
            //{
            //    { "a", 5 },
            //    { "b", true },
            //};
            //profileData.Add("objectExample", objectExample);

            Dictionary<string, object> objectExample = new Dictionary<string, object>();
            foreach (var pg in profile.ProfileGroups)
            {
                objectExample.Add(pg.GroupId.ToString(), pg.Group.Name);
            }
            profileData.Add("ProfileGroups", objectExample);

            //await docRef.SetAsync(profileData);
        }

        public async Task<List<Profile>> LoadProfiles()
        {
            List<Profile> profiles = new List<Profile>();
            CollectionReference usersRef = db.Collection(_firebaseProfilesCollection);
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
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

        public object AuthExplicit(string projectId, string jsonPath)
        {
            // Explicitly use service account credentials by specifying
            // the private key file.
            var credential = GoogleCredential.FromFile(jsonPath);
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }

        public async void ReadDataFromFireBase()
        {
            Task<List<Profile>> waitProfile = LoadProfiles();
            List<Profile> profiles = await waitProfile;
        }
    }
}