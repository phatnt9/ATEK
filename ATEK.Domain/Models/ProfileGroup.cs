using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Domain.Models
{
    public class ProfileGroup
    {
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}