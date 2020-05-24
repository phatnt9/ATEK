using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.ViewModels
{
    public class MainViewModel
    {
        private ObservableCollection<Profile> _profiles;
        private IProfilesRepository _profilesRepository = new ProfilesRepository();

        public MainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }
            Profiles = new ObservableCollection<Profile>(_profilesRepository.GetProfilesAsync().Result);
        }

        public ObservableCollection<Profile> Profiles
        {
            get
            {
                return _profiles;
            }
            set
            {
                _profiles = value;
            }
        }
    }
}