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
        private List<Class> allClasses;
        private List<Group> allGroups;
        private ObservableCollection<Profile> profiles;
        private Profile selectedProfile;
        private string searchInput;
        private ObservableCollection<Class> classes;
        private ObservableCollection<Group> groups;
        private string searchProfilesByClass;

        public ProfilesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddProfileCommand = new RelayCommand(OnAddProfile);
            RemoveProfilesCommand = new RelayCommand<object>(OnRemoveProfiles);
            RefreshProfilesCommand = new RelayCommand(OnRefreshProfiles);
            ImportProfilesCommand = new RelayCommand(OnImportProfiles);
        }

        #region Properties

        public string SearchInput
        {
            get { return searchInput; }
            set
            {
                SetProperty(ref searchInput, value);
                FilterCustomers(searchInput);
            }
        }

        public ObservableCollection<Profile> Profiles
        {
            get { return profiles; }
            set { SetProperty(ref profiles, value); }
        }

        public ObservableCollection<Class> Classes
        {
            get { return classes; }
            set { SetProperty(ref classes, value); }
        }

        public ObservableCollection<Group> Groups
        {
            get { return groups; }
            set { SetProperty(ref groups, value); }
        }

        public Profile SelectedProfile
        {
            get { return selectedProfile; }
            set { SetProperty(ref selectedProfile, value); }
        }

        public string SearchProfilesByClass
        {
            get { return searchProfilesByClass; }
            set { SetProperty(ref searchProfilesByClass, value); Console.WriteLine(value); }
        }

        #endregion Properties

        #region Methods

        private void FilterCustomers(string searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput))
            {
                Profiles = new ObservableCollection<Profile>(allProfiles);
                return;
            }
            else
            {
                Profiles = new ObservableCollection<Profile>(
                    allProfiles.Where(c =>
                    (
                    c.Name.ToLower().Contains(searchInput.ToLower()) ||
                    c.Pinno.ToLower().Contains(searchInput.ToLower()) ||
                    c.Adno.ToLower().Contains(searchInput.ToLower())
                    )
                    ));
            }
        }

        private void OnRefreshProfiles()
        {
            LoadProfiles();
        }

        public void LoadData()
        {
            Console.WriteLine("ProfilesView load data.");
            //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //{
            //    return;
            //}
            LoadProfiles();
            LoadClasses();
            LoadGroups();
        }

        private void LoadProfiles()
        {
            allProfiles = repo.GetProfiles().ToList();
            Console.WriteLine("Number of Profile:" + allProfiles.Count);
            Profiles = new ObservableCollection<Profile>(allProfiles);
        }

        private void LoadClasses()
        {
            allClasses = repo.GetClasses().ToList();
            Console.WriteLine("Number of Class:" + allClasses.Count);
            Classes = new ObservableCollection<Class>(allClasses);
        }

        private void LoadGroups()
        {
            allGroups = repo.GetGroups().ToList();
            Console.WriteLine("Number of Group:" + allGroups.Count);
            Groups = new ObservableCollection<Group>(allGroups);
        }

        private void OnAddProfile()
        {
            AddProfileRequested(new Profile());
        }

        private void OnImportProfiles()
        {
            ImportProfilesRequested();
        }

        private void OnRemoveProfiles(object obj)
        {
            if (obj != null)
            {
                System.Collections.IList items = (System.Collections.IList)obj;
                var collection = items.Cast<Profile>();
                if (collection.Count() > 0)
                {
                    repo.RemoveProfiles(collection);
                }
                LoadProfiles();
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