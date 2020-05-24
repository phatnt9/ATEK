using System.Collections.Generic;

namespace ATEK.Domain.Models
{
    public class Gate
    {
        public Gate()
        {
            ProfileGates = new List<ProfileGate>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public List<ProfileGate> ProfileGates { get; set; }
    }
}