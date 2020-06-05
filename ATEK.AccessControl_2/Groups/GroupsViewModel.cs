using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Groups
{
    public class GroupsViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private Group selectedGroup;
        private List<Class> allClasses;
        private List<Group> allGroups;
        private List<Profile> allGroupProfiles;
        private ObservableCollection<Class> classes;
        private ObservableCollection<Group> groups;
        private ObservableCollection<Profile> groupProfiles;

        private int searchGroupProfilesByClass;
        private string searchGroupProfilesInput;

        public GroupsViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddGroupCommand = new RelayCommand(OnAddGroup);
            EditGroupCommand = new RelayCommand<Group>(OnEditGroup);
            ManageGroupCommand = new RelayCommand<Group>(OnManageGroup);
            RemoveGroupCommand = new RelayCommand<Group>(OnRemoveGroup);
        }

        //=====================================================================

        #region Commands

        public RelayCommand AddGroupCommand { get; private set; }
        public RelayCommand<Group> EditGroupCommand { get; private set; }
        public RelayCommand<Group> ManageGroupCommand { get; private set; }
        public RelayCommand<Group> RemoveGroupCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        #endregion Commands

        //=====================================================================

        #region Actions

        public event Action<Group> AddGroupRequested = delegate { };

        public event Action<Group> EditGroupRequested = delegate { };

        public event Action<Group> ManageGroupRequested = delegate { };

        #endregion Actions

        //=====================================================================

        #region Properties

        public ObservableCollection<Group> Groups
        {
            get { return groups; }
            set { SetProperty(ref groups, value); }
        }

        public Group SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                SetProperty(ref selectedGroup, value);
                if (selectedGroup != null)
                {
                    LoadGroupProfiles(selectedGroup.Id);
                }
            }
        }

        public ObservableCollection<Profile> GroupProfiles
        {
            get { return groupProfiles; }
            set { SetProperty(ref groupProfiles, value); }
        }

        public int SearchGroupProfilesByClass
        {
            get { return searchGroupProfilesByClass; }
            set
            {
                SetProperty(ref searchGroupProfilesByClass, value);
                FilterGroupProfiles(searchGroupProfilesInput, searchGroupProfilesByClass);
            }
        }

        public string SearchGroupProfilesInput
        {
            get { return searchGroupProfilesInput; }
            set
            {
                SetProperty(ref searchGroupProfilesInput, value);
                FilterGroupProfiles(searchGroupProfilesInput, searchGroupProfilesByClass);
            }
        }

        public ObservableCollection<Class> Classes
        {
            get { return classes; }
            set { SetProperty(ref classes, value); }
        }

        #endregion Properties

        //=====================================================================

        #region Methods

        public void LoadData()
        {
            LoadClasses();
            LoadGroups();
            if (selectedGroup != null)
            {
                LoadGroupProfiles(selectedGroup.Id);
            }
        }

        private void LoadClasses()
        {
            allClasses = repo.GetClasses().ToList();
            allClasses.Insert(0, new Class() { Id = 0, Name = "All" });
            Classes = new ObservableCollection<Class>(allClasses);
            FilterGroupProfiles(searchGroupProfilesInput, searchGroupProfilesByClass);
        }

        private void LoadGroups()
        {
            allGroups = repo.GetGroups().ToList();
            Groups = new ObservableCollection<Group>(allGroups);
        }

        private void LoadGroupProfiles(int groupId)
        {
            allGroupProfiles = repo.LoadGroupProfiles(groupId).ToList();
            GroupProfiles = new ObservableCollection<Profile>(allGroupProfiles);
            FilterGroupProfiles(searchGroupProfilesInput, searchGroupProfilesByClass);
        }

        private void FilterGroupProfiles(string searchInput, int classId)
        {
            if (allGroupProfiles != null)
            {
                if (SelectedGroup != null && GroupProfiles != null)
                {
                    if (string.IsNullOrWhiteSpace(searchInput))
                    {
                        if (classId == 0)
                        {
                            GroupProfiles = new ObservableCollection<Profile>(allGroupProfiles);
                            return;
                        }
                        else
                        {
                            GroupProfiles = new ObservableCollection<Profile>(
                          allGroupProfiles.Where(c =>
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
                            GroupProfiles = new ObservableCollection<Profile>(
                           allGroupProfiles.Where(c =>
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
                            GroupProfiles = new ObservableCollection<Profile>(
                           allGroupProfiles.Where(c =>
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
        }

        private void OnAddGroup()
        {
            AddGroupRequested(new Group());
        }

        private void OnEditGroup(Group group)
        {
            EditGroupRequested(group);
        }

        private void OnManageGroup(Group group)
        {
            ManageGroupRequested(group);
        }

        private void OnRemoveGroup(Group group)
        {
            List<Group> deletes = new List<Group>();
            deletes.Add(group);
            repo.RemoveGroups(deletes);
            LoadData();
        }

        #endregion Methods

        //=====================================================================
    }
}