using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ATEK.Domain.Models
{
    public class Profile
    {
        public Profile()
        {
            ProfileGates = new List<ProfileGate>();
            ProfileGroups = new List<ProfileGroup>();
        }

        public int Id { get; set; }
        public int Pinno { get; set; }
        public int Adno { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
        public DateTime DateToLock { get; set; }
        public bool CheckDateToLock { get; set; }
        public string LicensePlate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public List<ProfileGate> ProfileGates { get; set; }
        public List<ProfileGroup> ProfileGroups { get; set; }
    }
}