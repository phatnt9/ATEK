using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Domain.Models
{
    public class ProfileGate
    {
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int GateId { get; set; }
        public Gate Gate { get; set; }
    }
}