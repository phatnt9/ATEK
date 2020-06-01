﻿using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATEK.AccessControl_2.Profiles;
using System.Timers;
using Unity;
using ATEK.AccessControl_2.Gates;
using ATEK.AccessControl_2.Classes;
using ATEK.AccessControl_2.Groups;

namespace ATEK.AccessControl_2.Main
{
    public class MainViewModel : BindableBase
    {
        private BindableBase currentViewModel;
        private ProfilesViewModel profilesViewModel;
        private ImportProfilesViewModel importProfilesViewModel;
        private GatesViewModel gatesViewModel;
        private ClassesViewModel classesViewModel;
        private GroupsViewModel groupsViewModel;
        private ManageGroupViewModel manageGroupViewModel;
        private bool isBackGroundWorkerBusy;
        private AddEditProfileViewModel addEditProfileViewModel;
        private AddEditClassViewModel addEditClassViewModel;
        private AddEditGroupViewModel addEditGroupViewModel;

        public MainViewModel()
        {
            NavToProfilesCommand = new RelayCommand(OnNavProfiles);
            NavToGatesCommand = new RelayCommand(OnNavGates);
            NavToClassesCommand = new RelayCommand(OnNavClasses);
            NavToGroupsCommand = new RelayCommand(OnNavGroups);

            ProfilesViewModel = ContainerHelper.Container.Resolve<ProfilesViewModel>();
            ProfilesViewModel.ImportProfilesRequested += NavToImportProfiles;
            ProfilesViewModel.AddProfileRequested += NavToAddProfiles;
            ProfilesViewModel.EditProfileRequested += NavToEditProfiles;

            GatesViewModel = ContainerHelper.Container.Resolve<GatesViewModel>();

            ClassesViewModel = ContainerHelper.Container.Resolve<ClassesViewModel>();
            ClassesViewModel.AddClassRequested += NavToAddClass;
            ClassesViewModel.EditClassRequested += NavToEditClass;

            GroupsViewModel = ContainerHelper.Container.Resolve<GroupsViewModel>();
            GroupsViewModel.AddGroupRequested += NavToAddGroup;
            GroupsViewModel.EditGroupRequested += NavToEditGroup;
            GroupsViewModel.ManageGroupRequested += NavToManageGroup;

            AddEditProfileViewModel = ContainerHelper.Container.Resolve<AddEditProfileViewModel>();
            AddEditProfileViewModel.Done += OnNavProfiles;

            AddEditClassViewModel = ContainerHelper.Container.Resolve<AddEditClassViewModel>();
            AddEditClassViewModel.Done += OnNavClasses;

            AddEditGroupViewModel = ContainerHelper.Container.Resolve<AddEditGroupViewModel>();
            AddEditGroupViewModel.Done += OnNavGroups;

            ManageGroupViewModel = ContainerHelper.Container.Resolve<ManageGroupViewModel>();
            ManageGroupViewModel.Done += OnNavGroups;
            ManageGroupViewModel.StartBackgroundProgress += OnStartProgress;
            ManageGroupViewModel.StopBackgroundProgress += OnStopProgress;

            ImportProfilesViewModel = ContainerHelper.Container.Resolve<ImportProfilesViewModel>();
            ImportProfilesViewModel.Done += OnNavProfiles;
            ImportProfilesViewModel.StartBackgroundProgress += OnStartProgress;
            ImportProfilesViewModel.StopBackgroundProgress += OnStopProgress;
        }

        //=====================================================================

        #region Commands

        public RelayCommand NavToProfilesCommand { get; private set; }
        public RelayCommand NavToGatesCommand { get; private set; }
        public RelayCommand NavToClassesCommand { get; private set; }
        public RelayCommand NavToGroupsCommand { get; private set; }

        #endregion Commands

        //=====================================================================

        //=====================================================================

        #region Methods

        private void NavToAddProfiles(Profile profile)
        {
            AddEditProfileViewModel.EditMode = false;
            AddEditProfileViewModel.SetProfile(profile);
            CurrentViewModel = AddEditProfileViewModel;
        }

        private void NavToEditProfiles(Profile profile)
        {
            AddEditProfileViewModel.EditMode = true;
            AddEditProfileViewModel.SetProfile(profile);
            CurrentViewModel = AddEditProfileViewModel;
        }

        private void OnStartProgress()
        {
            IsBackGroundWorkerBusy = true;
        }

        private void OnStopProgress()
        {
            IsBackGroundWorkerBusy = false;
        }

        private void NavToAddClass(Class @class)
        {
            AddEditClassViewModel.EditMode = false;
            AddEditClassViewModel.SetClass(@class);
            CurrentViewModel = AddEditClassViewModel;
        }

        private void NavToEditClass(Class @class)
        {
            AddEditClassViewModel.EditMode = true;
            AddEditClassViewModel.SetClass(@class);
            CurrentViewModel = AddEditClassViewModel;
        }

        private void NavToAddGroup(Group group)
        {
            AddEditGroupViewModel.EditMode = false;
            AddEditGroupViewModel.SetGroup(group);
            CurrentViewModel = AddEditGroupViewModel;
        }

        private void NavToEditGroup(Group group)
        {
            AddEditGroupViewModel.EditMode = true;
            AddEditGroupViewModel.SetGroup(group);
            CurrentViewModel = AddEditGroupViewModel;
        }

        private void NavToManageGroup(Group group)
        {
            ManageGroupViewModel.SetGroup(group);
            CurrentViewModel = ManageGroupViewModel;
        }

        public void LoadData()
        {
            CurrentViewModel = ProfilesViewModel;
        }

        private void NavToImportProfiles()
        {
            CurrentViewModel = ImportProfilesViewModel;
        }

        private void OnNavProfiles()
        {
            CurrentViewModel = ProfilesViewModel;
        }

        private void OnNavGates()
        {
            CurrentViewModel = GatesViewModel;
        }

        private void OnNavClasses()
        {
            CurrentViewModel = ClassesViewModel;
        }

        private void OnNavGroups()
        {
            CurrentViewModel = GroupsViewModel;
        }

        #endregion Methods

        //=====================================================================

        #region Properties

        public bool IsBackGroundWorkerBusy
        {
            get { return isBackGroundWorkerBusy; }
            set { SetProperty(ref isBackGroundWorkerBusy, value); }
        }

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set { SetProperty(ref currentViewModel, value); }
        }

        public AddEditClassViewModel AddEditClassViewModel
        {
            get { return addEditClassViewModel; }
            set { SetProperty(ref addEditClassViewModel, value); }
        }

        public AddEditProfileViewModel AddEditProfileViewModel
        {
            get { return addEditProfileViewModel; }
            set { SetProperty(ref addEditProfileViewModel, value); }
        }

        public AddEditGroupViewModel AddEditGroupViewModel
        {
            get { return addEditGroupViewModel; }
            set { SetProperty(ref addEditGroupViewModel, value); }
        }

        public ManageGroupViewModel ManageGroupViewModel
        {
            get { return manageGroupViewModel; }
            set { SetProperty(ref manageGroupViewModel, value); }
        }

        public ProfilesViewModel ProfilesViewModel
        {
            get { return profilesViewModel; }
            set { SetProperty(ref profilesViewModel, value); }
        }

        public ImportProfilesViewModel ImportProfilesViewModel
        {
            get { return importProfilesViewModel; }
            set { SetProperty(ref importProfilesViewModel, value); }
        }

        public GatesViewModel GatesViewModel
        {
            get { return gatesViewModel; }
            set { SetProperty(ref gatesViewModel, value); }
        }

        public ClassesViewModel ClassesViewModel
        {
            get { return classesViewModel; }
            set { SetProperty(ref classesViewModel, value); }
        }

        public GroupsViewModel GroupsViewModel
        {
            get { return groupsViewModel; }
            set { SetProperty(ref groupsViewModel, value); }
        }

        #endregion Properties

        //=====================================================================
    }
}