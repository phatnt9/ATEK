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
        private List<Group> allGroups;
        private List<Class> allClasses;
        private List<Group> allProfileGroups;
        private List<Gate> allProfileGates;
        private ObservableCollection<Profile> profiles;
        private ObservableCollection<Group> groups;
        private ObservableCollection<Class> classes;
        private ObservableCollection<Group> profileGroups;
        private ObservableCollection<Gate> profileGates;

        private Profile selectedProfile;
        private Group groupAddToProfile;

        private string searchProfilesInput;
        private int searchProfilesByClass;

        public ProfilesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddProfileCommand = new RelayCommand(OnAddProfile);
            AddGroupToProfileCommand = new RelayCommand(OnAddGroupToProfile, CanAddGroupToProfile);
            RemoveGroupFromProfileCommand = new RelayCommand<Group>(OnRemoveGroupFromProfile, CanRemoveGroupFromProfile);
            EditProfileCommand = new RelayCommand<Profile>(OnEditProfile);
            RemoveProfilesCommand = new RelayCommand<object>(OnRemoveProfiles);
            RefreshProfilesCommand = new RelayCommand(OnRefreshProfiles);
            ImportProfilesCommand = new RelayCommand(OnImportProfiles);
        }

        //=====================================================================

        #region Commands

        public RelayCommand AddProfileCommand { get; private set; }
        public RelayCommand AddGroupToProfileCommand { get; private set; }
        public RelayCommand<Group> RemoveGroupFromProfileCommand { get; private set; }
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
            if (allProfiles != null)
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
                       c.Gender.ToLower().Contains(searchInput.ToLower()) ||
                       c.Email.ToLower().Contains(searchInput.ToLower()) ||
                       c.Address.ToLower().Contains(searchInput.ToLower()) ||
                       c.Phone.ToLower().Contains(searchInput.ToLower()) ||
                       c.LicensePlate.ToLower().Contains(searchInput.ToLower()) ||
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
        }

        private void OnRefreshProfiles()
        {
            LoadProfiles();
        }

        public void LoadData()
        {
            LoadProfiles();
            LoadGroups();
            LoadClasses();
            LoadProfileGroupsAndGates();
        }

        private void LoadProfiles()
        {
            allProfiles = repo.GetProfiles().ToList();
            if (allProfiles.Count == 0)
            {
                SelectedProfile = null;
            }
            Profiles = new ObservableCollection<Profile>(allProfiles);
        }

        private void LoadGroups()
        {
            allGroups = repo.GetGroups().ToList();
            if (allGroups.Count == 0)
            {
                GroupAddToProfile = null;
            }
            Groups = new ObservableCollection<Group>(allGroups);
        }

        private void LoadClasses()
        {
            allClasses = repo.GetClasses().ToList();
            allClasses.Insert(0, new Class() { Id = 0, Name = "All" });
            Classes = new ObservableCollection<Class>(allClasses);
            FilterProfiles(searchProfilesInput, searchProfilesByClass);
        }

        private void OnAddProfile()
        {
            AddProfileRequested(new Profile());
        }

        private void OnAddGroupToProfile()
        {
            if (!allProfileGroups.Exists(g => (g.Id == groupAddToProfile.Id)))
            {
                if (!repo.AddProfileToGroup(groupAddToProfile, selectedProfile))
                {
                    Console.WriteLine("Add Group vao Profile khong thanh cong.");
                }
                else
                {
                    if (selectedProfile != null)
                    {
                        LoadProfileGroupsAndGates();
                    }
                }
            }
            else
            {
                Console.WriteLine("Group nay da ton tai.");
            }
        }

        private bool CanAddGroupToProfile()
        {
            bool result = (selectedProfile != null) && (groupAddToProfile != null);
            return result;
        }

        private void OnRemoveGroupFromProfile(Group profileGroup)
        {
            if (!repo.RemoveProfileFromGroup(profileGroup, selectedProfile))
            {
                Console.WriteLine("Remove Profile khong thanh cong.");
            }
            else
            {
                if (selectedProfile != null)
                {
                    LoadProfileGroupsAndGates();
                }
            }
        }

        private bool CanRemoveGroupFromProfile(Group profileGroup)
        {
            bool result = (selectedProfile != null);
            return result;
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

        public void LoadProfileGroupsAndGates()
        {
            if (selectedProfile != null)
            {
                allProfileGroups = repo.LoadGroupsOfProfile(selectedProfile.Id).ToList();
                allProfileGates = repo.LoadGatesOfProfile(selectedProfile.Id).ToList();
                ProfileGroups = new ObservableCollection<Group>(allProfileGroups);
                ProfileGates = new ObservableCollection<Gate>(allProfileGates);
                Console.WriteLine($"Select profile has {selectedProfile.ProfileGroups.Count} groups and {selectedProfile.ProfileGates.Count} gates");
            }
        }

        #endregion Methods

        //=====================================================================

        #region Properties

        public ObservableCollection<Group> ProfileGroups
        {
            get { return profileGroups; }
            set { SetProperty(ref profileGroups, value); }
        }

        public ObservableCollection<Gate> ProfileGates
        {
            get { return profileGates; }
            set { SetProperty(ref profileGates, value); }
        }

        public Group GroupAddToProfile
        {
            get { return groupAddToProfile; }
            set
            {
                SetProperty(ref groupAddToProfile, value);
                AddGroupToProfileCommand.RaiseCanExecuteChanged();
            }
        }

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

        public ObservableCollection<Group> Groups
        {
            get { return groups; }
            set { SetProperty(ref groups, value); }
        }

        public ObservableCollection<Class> Classes
        {
            get { return classes; }
            set { SetProperty(ref classes, value); }
        }

        public Profile SelectedProfile
        {
            get { return selectedProfile; }
            set
            {
                SetProperty(ref selectedProfile, value);
                if (selectedProfile != null)
                {
                    LoadProfileGroupsAndGates();
                }
                AddGroupToProfileCommand.RaiseCanExecuteChanged();
                RemoveGroupFromProfileCommand.RaiseCanExecuteChanged();
            }
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