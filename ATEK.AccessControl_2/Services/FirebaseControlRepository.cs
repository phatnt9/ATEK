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

        public async void ReadData()
        {
            CollectionReference usersRef = db.Collection("users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Console.WriteLine("User: {0}", document.Id);
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                Console.WriteLine("First: {0}", documentDictionary["First"]);
                if (documentDictionary.ContainsKey("Middle"))
                {
                    Console.WriteLine("Middle: {0}", documentDictionary["Middle"]);
                }
                Console.WriteLine("Last: {0}", documentDictionary["Last"]);
                Console.WriteLine("Born: {0}", documentDictionary["Born"]);
                Console.WriteLine();
            }
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

        public void ReadDataFromFireBase()
        {
            ReadData();
        }
    }
}