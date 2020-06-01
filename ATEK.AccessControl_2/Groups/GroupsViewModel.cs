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
        private List<Group> allGroups;
        private ObservableCollection<Group> groups;
        private Group selectedGroup;
        private ObservableCollection<Profile> groupProfiles;

        public GroupsViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddGroupCommand = new RelayCommand(OnAddGroup);
            EditGroupCommand = new RelayCommand<Group>(OnEditGroup);
            RemoveGroupCommand = new RelayCommand<Group>(OnRemoveGroup);
            ManageGroupCommand = new RelayCommand<Group>(OnManageGroup);
        }

        #region Actions

        public event Action<Group> AddGroupRequested = delegate { };

        public event Action<Group> EditGroupRequested = delegate { };

        public event Action<Group> ManageGroupRequested = delegate { };

        #endregion Actions

        #region Commands

        public RelayCommand AddGroupCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand<Group> EditGroupCommand { get; private set; }
        public RelayCommand<Group> ManageGroupCommand { get; private set; }
        public RelayCommand<Group> RemoveGroupCommand { get; private set; }

        #endregion Commands

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

        public void LoadData()
        {
            Console.WriteLine("GroupsView load data.");
            //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //{
            //    return;
            //}
            allGroups = repo.GetGroups().ToList();
            Groups = new ObservableCollection<Group>(allGroups);
            if (selectedGroup != null)
            {
                LoadGroupProfiles(selectedGroup.Id);
            }
        }

        private void LoadGroupProfiles(int groupId)
        {
            GroupProfiles = new ObservableCollection<Profile>(repo.LoadGroupProfiles(groupId));
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
    }
}