using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Gates
{
    public class SimpleEditableGate : ValidatableBindableBase
    {
        private int id;
        private string name;
        private string status;
        private string note;

        public int Id { get { return id; } set { SetProperty(ref id, value); } }

        [Required]
        public string Name { get { return name; } set { SetProperty(ref name, value); } }

        public string Status { get { return status; } set { SetProperty(ref status, value); } }

        public string Note { get { return note; } set { SetProperty(ref note, value); } }
    }
}