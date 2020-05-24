using System.Collections.Generic;

namespace ATEK.Domain.Models
{
    public class Group
    {
        public Group()
        {
            ProfileGroups = new List<ProfileGroup>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProfileGroup> ProfileGroups { get; set; }
    }
}