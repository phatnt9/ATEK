using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Domain.Models
{
    [FirestoreData]
    public class Class : ModelBase
    {
        private int id;
        private string name;
        private List<Profile> profiles;

        public Class()
        {
            Profiles = new List<Profile>();
        }

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        [FirestoreProperty]
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public List<Profile> Profiles
        {
            get { return profiles; }
            set { SetProperty(ref profiles, value); }
        }
    }
}