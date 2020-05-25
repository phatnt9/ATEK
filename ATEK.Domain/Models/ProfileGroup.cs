using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Domain.Models
{
    public class ProfileGroup : ModelBase
    {
        private int profileId;
        private Profile profile;
        private int groupId;
        private Group group;

        public int ProfileId { get { return profileId; } set { SetProperty(ref profileId, value); } }
        public Profile Profile { get { return profile; } set { SetProperty(ref profile, value); } }
        public int GroupId { get { return groupId; } set { SetProperty(ref groupId, value); } }
        public Group Group { get { return group; } set { SetProperty(ref group, value); } }
    }
}