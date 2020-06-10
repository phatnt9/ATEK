using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace ATEK.Domain.Models
{
    [FirestoreData]
    public class Gate : ModelBase
    {
        private int id;
        private string name;
        private string status;
        private string note;
        private List<ProfileGate> profileGates;
        private string firebaseId;

        public Gate()
        {
            ProfileGates = new List<ProfileGate>();
        }

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public string FirebaseId
        {
            get { return firebaseId; }
            set { SetProperty(ref firebaseId, value); }
        }

        [FirestoreProperty]
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        [FirestoreProperty]
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        [FirestoreProperty]
        public string Note
        {
            get { return note; }
            set { SetProperty(ref note, value); }
        }

        public List<ProfileGate> ProfileGates
        {
            get { return profileGates; }
            set { SetProperty(ref profileGates, value); }
        }
    }
}