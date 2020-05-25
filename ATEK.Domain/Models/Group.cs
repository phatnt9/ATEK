using System.Collections.Generic;

namespace ATEK.Domain.Models
{
    public class Group : ModelBase
    {
        private int id;
        private string name;
        private List<ProfileGroup> profileGroups;

        public Group()
        {
            ProfileGroups = new List<ProfileGroup>();
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

        public List<ProfileGroup> ProfileGroups
        {
            get { return profileGroups; }
            set { SetProperty(ref profileGroups, value); }
        }
    }
}