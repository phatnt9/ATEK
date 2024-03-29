﻿using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ATEK.AccessControl_2.Profiles
{
    public class ProfilesViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;

        private Profile selectedProfile;
        private List<Profile> allProfiles;
        private ObservableCollection<Profile> profiles;

        private Gate gateAddToProfile;
        private List<Gate> allGates;
        private List<Gate> allProfileGates;
        private ObservableCollection<Gate> gates;
        private ObservableCollection<Gate> profileGates;

        private Group groupAddToProfile;
        private List<Group> allGroups;
        private List<Group> allProfileGroups;
        private ObservableCollection<Group> groups;
        private ObservableCollection<Group> profileGroups;

        private List<Class> allClasses;
        private ObservableCollection<Class> classes;

        private int searchProfilesByClass;
        private string searchProfilesInput;

        public ProfilesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            FirebaseOnCommand = new RelayCommand(OnFirebaseCommand);
            FirebaseOffCommand = new RelayCommand(OffFirebaseCommand);

            AddProfileCommand = new RelayCommand(OnAddProfile);
            EditProfileCommand = new RelayCommand<Profile>(OnEditProfile);
            RemoveProfilesCommand = new RelayCommand<object>(OnRemoveProfiles);
            RefreshProfilesCommand = new RelayCommand(OnRefreshProfiles);
            ImportProfilesCommand = new RelayCommand(OnImportProfiles);

            AddGroupToProfileCommand = new RelayCommand(OnAddGroupToProfile, CanAddGroupToProfile);
            RemoveGroupFromProfileCommand = new RelayCommand<Group>(OnRemoveGroupFromProfile, CanRemoveGroupFromProfile);

            AddGateToProfileCommand = new RelayCommand(OnAddGateToProfile, CanAddGateToProfile);
            RemoveGateFromProfileCommand = new RelayCommand<Gate>(OnRemoveGateFromProfile, CanRemoveGateFromProfile);
        }

        //=====================================================================

        #region Commands

        public RelayCommand FirebaseOnCommand { get; private set; }
        public RelayCommand FirebaseOffCommand { get; private set; }
        public RelayCommand AddProfileCommand { get; private set; }
        public RelayCommand<Profile> EditProfileCommand { get; private set; }
        public RelayCommand<object> RemoveProfilesCommand { get; private set; }
        public RelayCommand RefreshProfilesCommand { get; private set; }
        public RelayCommand ImportProfilesCommand { get; private set; }

        public RelayCommand AddGateToProfileCommand { get; private set; }
        public RelayCommand<Gate> RemoveGateFromProfileCommand { get; private set; }

        public RelayCommand AddGroupToProfileCommand { get; private set; }
        public RelayCommand<Group> RemoveGroupFromProfileCommand { get; private set; }

        #endregion Commands

        //=====================================================================

        #region Actions

        public event Action<Profile> AddProfileRequested = delegate { };

        public event Action<Profile> EditProfileRequested = delegate { };

        public event Action ImportProfilesRequested = delegate { };

        #endregion Actions

        //=====================================================================

        #region Methods

        private void OnFirebaseCommand()
        {
            repo.Firebase_SetTime();
        }

        private void OffFirebaseCommand()
        {
            repo.Firebase_GetTime();
        }

        public void LoadData()
        {
            LoadProfiles();
            LoadClasses();
            LoadGates();
            LoadGroups();
            LoadProfileGroupsAndGates();
        }

        private void LoadProfiles()
        {
            allProfiles = repo.GetProfiles().ToList();
            if (allProfiles != null)
            {
                if (allProfiles.Count == 0)
                {
                    SelectedProfile = null;
                }
                Profiles = new ObservableCollection<Profile>(allProfiles);
            }
            else
            {
                SelectedProfile = null;
                Profiles = new ObservableCollection<Profile>();
            }
        }

        private void LoadClasses()
        {
            allClasses = repo.GetClasses().ToList();
            if (allClasses != null)
            {
                allClasses.Insert(0, new Class() { Id = 0, Name = "All" });
                Classes = new ObservableCollection<Class>(allClasses);
                FilterProfiles(searchProfilesInput, searchProfilesByClass);
            }
            else
            {
                Classes = new ObservableCollection<Class>();
            }
        }

        private void LoadGates()
        {
            allGates = repo.GetGates().ToList();
            if (allGates != null)
            {
                if (allGates.Count == 0)
                {
                    GateAddToProfile = null;
                }
                Gates = new ObservableCollection<Gate>(allGates);
            }
            else
            {
                GateAddToProfile = null;
                Gates = new ObservableCollection<Gate>();
            }
        }

        private void LoadGroups()
        {
            allGroups = repo.GetGroups().ToList();
            if (allGroups != null)
            {
                if (allGroups.Count == 0)
                {
                    GroupAddToProfile = null;
                }
                Groups = new ObservableCollection<Group>(allGroups);
            }
            else
            {
                GroupAddToProfile = null;
                Groups = new ObservableCollection<Group>();
            }
        }

        public void LoadProfileGroupsAndGates()
        {
            if (selectedProfile != null)
            {
                allProfileGroups = repo.LoadGroupsOfProfile(selectedProfile.Id).ToList();
                if (allProfileGroups != null)
                {
                    ProfileGroups = new ObservableCollection<Group>(allProfileGroups);
                }
                else
                {
                    ProfileGroups = new ObservableCollection<Group>();
                }
                allProfileGates = repo.LoadGatesOfProfile(selectedProfile.Id).ToList();
                if (allProfileGates != null)
                {
                    ProfileGates = new ObservableCollection<Gate>(allProfileGates);
                }
                else
                {
                    ProfileGates = new ObservableCollection<Gate>();
                }
                Console.WriteLine($"Select profile has {selectedProfile.ProfileGroups.Count} groups and {selectedProfile.ProfileGates.Count} gates");
            }
        }

        private void OnAddProfile()
        {
            AddProfileRequested(new Profile());
        }

        private void OnEditProfile(Profile profile)
        {
            EditProfileRequested(profile);
        }

        private void OnRemoveProfiles(object obj)
        {
            if (obj != null)
            {
                System.Collections.IList items = (System.Collections.IList)obj;
                var collection = items.Cast<Profile>();
                if (collection.Count() > 0)
                {
                    if (repo.RemoveProfiles(collection))
                    {
                        LoadProfiles();
                    }
                }
            }
        }

        private void OnRefreshProfiles()
        {
            LoadProfiles();
        }

        private void OnImportProfiles()
        {
            ImportProfilesRequested();
        }

        private bool CanAddGateToProfile()
        {
            bool result = (selectedProfile != null) && (gateAddToProfile != null);
            return result;
        }

        private void OnAddGateToProfile()
        {
            if (!allProfileGates.Exists(g => (g.Id == gateAddToProfile.Id)))
            {
                if (repo.Firebase_AddProfileGate(selectedProfile, gateAddToProfile))
                {
                    if (!repo.AddProfileGate(selectedProfile, gateAddToProfile))
                    {
                        MessageBox.Show("Error Adding Gate.");
                        repo.Firebase_RemoveProfileGateAsync(selectedProfile, gateAddToProfile);
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
                    MessageBox.Show("Error, Please check your internet.");
                }
            }
        }

        private bool CanRemoveGateFromProfile(Gate profileGate)
        {
            bool result = (selectedProfile != null);
            return result;
        }

        private void OnRemoveGateFromProfile(Gate profileGate)
        {
            if (repo.Firebase_RemoveProfileGate(selectedProfile, profileGate))
            {
                if (!repo.RemoveProfileGate(selectedProfile, profileGate))
                {
                    Console.WriteLine("Error Removing Gate.");
                    repo.Firebase_AddProfileGateAsync(selectedProfile, profileGate);
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
                MessageBox.Show("Error, please check your internet");
            }
        }

        private bool CanAddGroupToProfile()
        {
            bool result = (selectedProfile != null) && (groupAddToProfile != null);
            return result;
        }

        private void OnAddGroupToProfile()
        {
            if (!allProfileGroups.Exists(g => (g.Id == groupAddToProfile.Id)))
            {
                if (!repo.AddProfileGroup(selectedProfile, groupAddToProfile))
                {
                    Console.WriteLine("Error Adding Group.");
                }
                else
                {
                    if (selectedProfile != null)
                    {
                        LoadProfileGroupsAndGates();
                    }
                }
            }
        }

        private bool CanRemoveGroupFromProfile(Group profileGroup)
        {
            bool result = (selectedProfile != null);
            return result;
        }

        private void OnRemoveGroupFromProfile(Group profileGroup)
        {
            if (!repo.RemoveProfileGroup(selectedProfile, profileGroup))
            {
                Console.WriteLine("Error Remove Profile.");
            }
            else
            {
                if (selectedProfile != null)
                {
                    LoadProfileGroupsAndGates();
                }
            }
        }

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

        #endregion Methods

        //=====================================================================

        #region Properties

        public int SearchProfilesByClass
        {
            get { return searchProfilesByClass; }
            set
            {
                SetProperty(ref searchProfilesByClass, value);
                FilterProfiles(searchProfilesInput, searchProfilesByClass);
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

        public ObservableCollection<Profile> Profiles
        {
            get { return profiles; }
            set { SetProperty(ref profiles, value); }
        }

        public Gate GateAddToProfile
        {
            get { return gateAddToProfile; }
            set
            {
                SetProperty(ref gateAddToProfile, value);
                AddGateToProfileCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<Gate> Gates
        {
            get { return gates; }
            set { SetProperty(ref gates, value); }
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

        public ObservableCollection<Group> Groups
        {
            get { return groups; }
            set { SetProperty(ref groups, value); }
        }

        public ObservableCollection<Group> ProfileGroups
        {
            get { return profileGroups; }
            set { SetProperty(ref profileGroups, value); }
        }

        public ObservableCollection<Class> Classes
        {
            get { return classes; }
            set { SetProperty(ref classes, value); }
        }

        #endregion Properties

        //=====================================================================
    }
}