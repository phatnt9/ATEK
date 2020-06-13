using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ATEK.Domain.Models
{
    [FirestoreData]
    public class Profile : ModelBase
    {
        private int id;
        private string pinno;
        private string adno;
        private string name;
        private string gender;
        private DateTime dateOfBirth;
        private DateTime dateOfIssue;
        private string email;
        private string address;
        private string phone;
        private string status;
        private string image;
        private DateTime dateToLock;
        private bool checkDateToLock;
        private string licensePlate;
        private DateTime dateCreated;
        private DateTime dateModified;
        private Class @class;
        private int classId;
        private List<ProfileGate> profileGates;
        private List<ProfileGroup> profileGroups;

        public Profile()
        {
            ProfileGates = new List<ProfileGate>();
            ProfileGroups = new List<ProfileGroup>();
        }

        public int Id { get { return id; } set { SetProperty(ref id, value); } }

        [FirestoreProperty]
        public string Pinno { get { return pinno; } set { SetProperty(ref pinno, value); } }

        [FirestoreProperty]
        public string Adno { get { return adno; } set { SetProperty(ref adno, value); } }

        [FirestoreProperty]
        public string Name { get { return name; } set { SetProperty(ref name, value); } }

        [FirestoreProperty]
        public int ClassId { get { return classId; } set { SetProperty(ref classId, value); } }

        public string Gender { get { return gender; } set { SetProperty(ref gender, value); } }

        public DateTime DateOfBirth
        {
            get { return DateTime.SpecifyKind(dateOfBirth, DateTimeKind.Utc); }
            set { SetProperty(ref dateOfBirth, value); }
        }

        public DateTime DateOfIssue
        {
            get { return DateTime.SpecifyKind(dateOfIssue, DateTimeKind.Utc); }
            set { SetProperty(ref dateOfIssue, value); }
        }

        public string Email { get { return email; } set { SetProperty(ref email, value); } }
        public string Address { get { return address; } set { SetProperty(ref address, value); } }
        public string Phone { get { return phone; } set { SetProperty(ref phone, value); } }
        public string Status { get { return status; } set { SetProperty(ref status, value); } }

        [FirestoreProperty]
        public string Image { get { return image; } set { SetProperty(ref image, value); } }

        [FirestoreProperty]
        public DateTime DateToLock
        {
            get { return DateTime.SpecifyKind(dateToLock, DateTimeKind.Utc); }
            set { SetProperty(ref dateToLock, value); }
        }

        [FirestoreProperty]
        public bool CheckDateToLock { get { return checkDateToLock; } set { SetProperty(ref checkDateToLock, value); } }

        [FirestoreProperty]
        public string LicensePlate { get { return licensePlate; } set { SetProperty(ref licensePlate, value); } }

        public DateTime DateCreated
        {
            get { return DateTime.SpecifyKind(dateCreated, DateTimeKind.Utc); }
            set { SetProperty(ref dateCreated, value); }
        }

        public DateTime DateModified
        {
            get { return DateTime.SpecifyKind(dateModified, DateTimeKind.Utc); }
            set { SetProperty(ref dateModified, value); }
        }

        public Class Class { get { return @class; } set { SetProperty(ref @class, value); } }
        public List<ProfileGate> ProfileGates { get { return profileGates; } set { SetProperty(ref profileGates, value); } }
        public List<ProfileGroup> ProfileGroups { get { return profileGroups; } set { SetProperty(ref profileGroups, value); } }
    }
}