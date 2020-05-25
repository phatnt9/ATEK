using System.Collections.Generic;

namespace ATEK.Domain.Models
{
    public class Gate : ModelBase
    {
        private int id;
        private string name;
        private string status;
        private string note;
        private List<ProfileGate> profileGates;

        public Gate()
        {
            ProfileGates = new List<ProfileGate>();
        }

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

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