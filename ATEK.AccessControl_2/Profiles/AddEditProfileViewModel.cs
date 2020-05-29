using ATEK.AccessControl_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Profiles
{
    public class AddEditProfileViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private bool editMode;

        public AddEditProfileViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
        }

        public bool EditMode
        {
            get { return editMode; }
            set { SetProperty(ref editMode, value); }
        }
    }
}