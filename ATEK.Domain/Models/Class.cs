using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Domain.Models
{
    public class Class : ModelBase
    {
        private int id;
        private string name;
        private List<Profile> profiles;

        public Class()
        {
            Profiles = new List<Profile>();
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public List<Profile> Profiles { get => profiles; set => profiles = value; }
    }
}