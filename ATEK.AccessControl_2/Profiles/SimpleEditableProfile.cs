using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Profiles
{
    public class SimpleEditableProfile : ValidatableBindableBase
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

        public int Id { get { return id; } set { SetProperty(ref id, value); } }

        [Required]
        public string Pinno { get { return pinno; } set { SetProperty(ref pinno, value); } }

        [Required]
        public string Adno { get { return adno; } set { SetProperty(ref adno, value); } }

        [Required]
        public string Name { get { return name; } set { SetProperty(ref name, value); } }

        [Required]
        public string Gender { get { return gender; } set { SetProperty(ref gender, value); } }

        [Required]
        public DateTime DateOfBirth { get { return dateOfBirth; } set { SetProperty(ref dateOfBirth, value); } }

        [Required]
        public DateTime DateOfIssue { get { return dateOfIssue; } set { SetProperty(ref dateOfIssue, value); } }

        [EmailAddress]
        public string Email { get { return email; } set { SetProperty(ref email, value); } }

        [Required]
        public string Address { get { return address; } set { SetProperty(ref address, value); } }

        [Phone]
        public string Phone { get { return phone; } set { SetProperty(ref phone, value); } }

        [Required]
        public string Status { get { return status; } set { SetProperty(ref status, value); } }

        [Required]
        public string Image { get { return image; } set { SetProperty(ref image, value); } }

        [Required]
        public DateTime DateToLock { get { return dateToLock; } set { SetProperty(ref dateToLock, value); } }

        [Required]
        public bool CheckDateToLock { get { return checkDateToLock; } set { SetProperty(ref checkDateToLock, value); } }

        [Required]
        public string LicensePlate { get { return licensePlate; } set { SetProperty(ref licensePlate, value); } }

        [Required]
        public DateTime DateCreated { get { return dateCreated; } set { SetProperty(ref dateCreated, value); } }

        [Required]
        public DateTime DateModified { get { return dateModified; } set { SetProperty(ref dateModified, value); } }

        [Required]
        public Class Class { get { return @class; } set { SetProperty(ref @class, value); } }

        [Required]
        public int ClassId { get { return classId; } set { SetProperty(ref classId, value); } }
    }
}