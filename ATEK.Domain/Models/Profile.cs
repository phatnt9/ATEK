using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ATEK.Domain.Models
{
    public class Profile : ModelBase
    {
        private int id;
        private int pinno;
        private int adno;
        private string name;
        private string @class;
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
        private List<ProfileGate> profileGates;
        private List<ProfileGroup> profileGroups;

        public Profile()
        {
            ProfileGates = new List<ProfileGate>();
            ProfileGroups = new List<ProfileGroup>();
        }

        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public int Pinno { get { return pinno; } set { SetProperty(ref pinno, value); } }
        public int Adno { get { return adno; } set { SetProperty(ref adno, value); } }
        public string Name { get { return name; } set { SetProperty(ref name, value); } }
        public string Class { get { return @class; } set { SetProperty(ref @class, value); } }
        public string Gender { get { return gender; } set { SetProperty(ref gender, value); } }
        public DateTime DateOfBirth { get { return dateOfBirth; } set { SetProperty(ref dateOfBirth, value); } }
        public DateTime DateOfIssue { get { return dateOfIssue; } set { SetProperty(ref dateOfIssue, value); } }
        public string Email { get { return email; } set { SetProperty(ref email, value); } }
        public string Address { get { return address; } set { SetProperty(ref address, value); } }
        public string Phone { get { return phone; } set { SetProperty(ref phone, value); } }
        public string Status { get { return status; } set { SetProperty(ref status, value); } }
        public string Image { get { return image; } set { SetProperty(ref image, value); } }
        public DateTime DateToLock { get { return dateToLock; } set { SetProperty(ref dateToLock, value); } }
        public bool CheckDateToLock { get { return checkDateToLock; } set { SetProperty(ref checkDateToLock, value); } }
        public string LicensePlate { get { return licensePlate; } set { SetProperty(ref licensePlate, value); } }
        public DateTime DateCreated { get { return dateCreated; } set { SetProperty(ref dateCreated, value); } }
        public DateTime DateModified { get { return dateModified; } set { SetProperty(ref dateModified, value); } }

        public List<ProfileGate> ProfileGates { get { return profileGates; } set { SetProperty(ref profileGates, value); } }
        public List<ProfileGroup> ProfileGroups { get { return profileGroups; } set { SetProperty(ref profileGroups, value); } }
    }
}