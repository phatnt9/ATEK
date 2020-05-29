using ATEK.AccessControl_2.Services;
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

namespace ATEK.AccessControl_2.Main
{
    public class MainViewModel : BindableBase
    {
        private BindableBase currentViewModel;
        private ProfilesViewModel profilesViewModel;
        private ImportProfilesViewModel importProfilesViewModel;
        private GatesViewModel gatesViewModel;
        private ClassesViewModel classesViewModel;
        private AddEditClassViewModel addEditClassViewModel;

        public MainViewModel()
        {
            NavToProfilesCommand = new RelayCommand(OnNavProfiles);
            NavToGatesCommand = new RelayCommand(OnNavGates);
            NavToClassesCommand = new RelayCommand(OnNavClasses);

            ProfilesViewModel = ContainerHelper.Container.Resolve<ProfilesViewModel>();
            GatesViewModel = ContainerHelper.Container.Resolve<GatesViewModel>();
            ClassesViewModel = ContainerHelper.Container.Resolve<ClassesViewModel>();
            AddEditClassViewModel = ContainerHelper.Container.Resolve<AddEditClassViewModel>();
            ImportProfilesViewModel = ContainerHelper.Container.Resolve<ImportProfilesViewModel>();

            ProfilesViewModel.ImportProfilesRequested += NavToImportProfiles;
            ImportProfilesViewModel.Done += OnNavProfiles;
            ClassesViewModel.AddClassRequested += NavToAddClass;
            ClassesViewModel.EditClassRequested += NavToEditClass;
            AddEditClassViewModel.Done += OnNavClasses;
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

        public RelayCommand NavToProfilesCommand { get; private set; }
        public RelayCommand NavToGatesCommand { get; private set; }
        public RelayCommand NavToClassesCommand { get; private set; }

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
    }
}