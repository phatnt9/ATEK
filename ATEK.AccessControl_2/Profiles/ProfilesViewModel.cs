using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATEK.AccessControl_2.Profiles
{
    public class ProfilesViewModel : ViewModelBase
    {
        private IProfilesRepository _profilesRepository = new ProfilesRepository();
        private ObservableCollection<Profile> profiles;

        public ProfilesViewModel()
        {
        }

        public async void LoadDataAsync()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            Profiles = new ObservableCollection<Profile>(await _profilesRepository.GetProfilesAsync());
        }

        public void LoadData()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            Profiles = new ObservableCollection<Profile>(_profilesRepository.GetProfiles());
        }

        public ObservableCollection<Profile> Profiles
        {
            get { return profiles; }
            set { SetProperty(ref profiles, value); }
        }
    }
}