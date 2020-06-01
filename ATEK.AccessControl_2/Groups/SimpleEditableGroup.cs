using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Groups
{
    public class SimpleEditableGroup : ValidatableBindableBase
    {
        private int id;
        private string name;

        public int Id { get { return id; } set { SetProperty(ref id, value); } }

        [Required]
        public string Name { get { return name; } set { SetProperty(ref name, value); } }
    }
}