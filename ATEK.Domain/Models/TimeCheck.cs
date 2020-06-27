using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Domain.Models
{
    [FirestoreData]
    public class TimeCheck : ModelBase
    {
        private int id;
        private string pinno;
        private DateTime timecheck;
        private string gateFirebaseId;
        private string firebaseId;

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        [FirestoreProperty]
        public string Pinno
        {
            get { return pinno; }
            set { SetProperty(ref pinno, value); }
        }

        [FirestoreProperty]
        public string GateFirebaseId
        {
            get { return gateFirebaseId; }
            set { SetProperty(ref gateFirebaseId, value); }
        }

        public string FirebaseId
        {
            get { return firebaseId; }
            set { SetProperty(ref firebaseId, value); }
        }

        [FirestoreProperty]
        public DateTime Timecheck
        {
            get { return DateTime.SpecifyKind(timecheck, DateTimeKind.Utc); }
            set { SetProperty(ref timecheck, value); }
        }
    }
}