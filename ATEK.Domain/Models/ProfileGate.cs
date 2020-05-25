using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Domain.Models
{
    public class ProfileGate : ModelBase
    {
        private int profileId;
        private Profile profile;
        private int gateId;
        private Gate gate;

        public int ProfileId { get { return profileId; } set { SetProperty(ref profileId, value); } }
        public Profile Profile { get { return profile; } set { SetProperty(ref profile, value); } }
        public int GateId { get { return gateId; } set { SetProperty(ref gateId, value); } }
        public Gate Gate { get { return gate; } set { SetProperty(ref gate, value); } }
    }
}