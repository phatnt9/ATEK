using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Domain.Models
{
    public class ActiveTime : ModelBase
    {
        private int id;
        private string fromTime;
        private string toTime;
        private int profileGateProfileId;
        private int profileGateGateId;
        private ProfileGate profileGate;

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public string FromTime
        {
            get { return fromTime; }
            set { SetProperty(ref fromTime, value); }
        }

        public string ToTime
        {
            get { return toTime; }
            set { SetProperty(ref toTime, value); }
        }

        public int ProfileGateProfileId
        {
            get { return profileGateProfileId; }
            set { SetProperty(ref profileGateProfileId, value); }
        }

        public int ProfileGateGateId
        {
            get { return profileGateGateId; }
            set { SetProperty(ref profileGateGateId, value); }
        }

        public ProfileGate ProfileGate
        {
            get { return profileGate; }
            set { SetProperty(ref profileGate, value); }
        }
    }
}