using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ATEK.AccessControl_2.Profiles
{
    public class ProfilesViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private List<Profile> allProfiles;
        private ObservableCollection<Profile> profiles;
        private Profile selectedProfile;

        public ProfilesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddProfileCommand = new RelayCommand(OnAddProfile);
            RemoveProfilesCommand = new RelayCommand<object>(OnRemoveProfiles);
            RefreshProfilesCommand = new RelayCommand(OnRefreshProfiles);
            ImportProfilesCommand = new RelayCommand(OnImportProfiles);
        }

        #region Properties

        public ObservableCollection<Profile> Profiles
        {
            get { return profiles; }
            set { SetProperty(ref profiles, value); }
        }

        public Profile SelectedProfile
        {
            get { return selectedProfile; }
            set { SetProperty(ref selectedProfile, value); }
        }

        #endregion Properties

        #region Methods

        private void OnRefreshProfiles()
        {
            LoadData();
        }

        public void LoadData()
        {
            Console.WriteLine("ProfilesView load data.");
            //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //{
            //    return;
            //}
            allProfiles = repo.GetProfiles();
            Profiles = new ObservableCollection<Profile>(allProfiles);
        }

        private void OnAddProfile()
        {
            AddProfileRequested(new Profile());
        }

        private void OnImportProfiles()
        {
            ImportProfilesRequested();
        }

        private void OnRemoveProfiles(object profiles)
        {
            if (profiles != null)
            {
                System.Collections.IList items = (System.Collections.IList)profiles;
                var collection = items.Cast<Profile>();
                if (collection.Count() > 0)
                {
                    repo.RemoveProfiles(collection);
                }
                LoadData();
            }
        }

        #endregion Methods

        #region Commands

        public RelayCommand AddProfileCommand { get; private set; }
        public RelayCommand<Profile> EditProfileCommand { get; private set; }
        public RelayCommand ImportProfilesCommand { get; private set; }
        public RelayCommand<object> RemoveProfilesCommand { get; private set; }
        public RelayCommand RefreshProfilesCommand { get; private set; }

        #endregion Commands

        #region Actions

        public event Action<Profile> AddProfileRequested = delegate { };

        public event Action ImportProfilesRequested = delegate { };

        #endregion Actions
    }
}