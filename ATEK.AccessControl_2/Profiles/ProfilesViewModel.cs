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
        private ObservableCollection<Profile> profiles;
        private Profile selectedProfile;
        private string searchProfilesInput;
        private ObservableCollection<Class> classes;
        private int searchProfilesByClass;

        public ProfilesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddProfileCommand = new RelayCommand(OnAddProfile);
            EditProfileCommand = new RelayCommand<Profile>(OnEditProfile);
            RemoveProfilesCommand = new RelayCommand<object>(OnRemoveProfiles);
            RefreshProfilesCommand = new RelayCommand(OnRefreshProfiles);
            ImportProfilesCommand = new RelayCommand(OnImportProfiles);
        }

        //=====================================================================

        #region Commands

        public RelayCommand AddProfileCommand { get; private set; }
        public RelayCommand<Profile> EditProfileCommand { get; private set; }
        public RelayCommand<object> RemoveProfilesCommand { get; private set; }
        public RelayCommand RefreshProfilesCommand { get; private set; }
        public RelayCommand ImportProfilesCommand { get; private set; }

        #endregion Commands

        //=====================================================================

        #region Actions

        public event Action<Profile> AddProfileRequested = delegate { };

        public event Action<Profile> EditProfileRequested = delegate { };

        public event Action ImportProfilesRequested = delegate { };

        #endregion Actions

        //=====================================================================

        #region Methods

        private void FilterProfiles(string searchInput, int classId)
        {
            if (string.IsNullOrWhiteSpace(searchInput))
            {
                if (classId == 0)
                {
                    Profiles = new ObservableCollection<Profile>(allProfiles);
                    return;
                }
                else
                {
                    Profiles = new ObservableCollection<Profile>(
                  allProfiles.Where(c =>
                  (
                  c.ClassId == classId
                  )
                  ));
                }
            }
            else
            {
                if (classId == 0)
                {
                    Profiles = new ObservableCollection<Profile>(
                   allProfiles.Where(c =>
                   (
                   (
                   c.Name.ToLower().Contains(searchInput.ToLower()) ||
                   c.Pinno.ToLower().Contains(searchInput.ToLower()) ||
                   c.Adno.ToLower().Contains(searchInput.ToLower())
                   )
                   )
                   ));
                }
                else
                {
                    Profiles = new ObservableCollection<Profile>(
                   allProfiles.Where(c =>
                   (
                    c.ClassId == classId &&
                   (
                   c.Name.ToLower().Contains(searchInput.ToLower()) ||
                   c.Pinno.ToLower().Contains(searchInput.ToLower()) ||
                   c.Adno.ToLower().Contains(searchInput.ToLower())
                   )
                   )
                   ));
                }
            }
        }

        private void OnRefreshProfiles()
        {
            LoadProfiles();
        }

        public void LoadData()
        {
            LoadProfiles();
            LoadClasses();
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
            allClasses.Insert(0, new Class() { Id = 0, Name = "All" });
            Classes = new ObservableCollection<Class>(allClasses);
        }

        private void OnAddProfile()
        {
            AddProfileRequested(new Profile());
        }

        private void OnEditProfile(Profile profile)
        {
            EditProfileRequested(profile);
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

        //=====================================================================

        #region Properties

        public string SearchProfilesInput
        {
            get { return searchProfilesInput; }
            set
            {
                SetProperty(ref searchProfilesInput, value);
                FilterProfiles(searchProfilesInput, searchProfilesByClass);
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

        public Profile SelectedProfile
        {
            get { return selectedProfile; }
            set { SetProperty(ref selectedProfile, value); }
        }

        public int SearchProfilesByClass
        {
            get { return searchProfilesByClass; }
            set
            {
                SetProperty(ref searchProfilesByClass, value);
                FilterProfiles(searchProfilesInput, searchProfilesByClass);
            }
        }

        #endregion Properties

        //=====================================================================
    }
}