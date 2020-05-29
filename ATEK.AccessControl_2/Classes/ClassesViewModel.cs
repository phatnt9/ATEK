using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATEK.AccessControl_2.Classes
{
    public class ClassesViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private List<Class> allClasses;
        private ObservableCollection<Class> classes;
        private Class selectedClass;
        private ObservableCollection<Profile> classProfiles;

        public ClassesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddClassCommand = new RelayCommand(OnAddClass);
            EditClassCommand = new RelayCommand<Class>(OnEditClass);
        }

        #region Actions

        public event Action<Class> AddClassRequested = delegate { };

        public event Action<Class> EditClassRequested = delegate { };

        #endregion Actions

        #region Commands

        public RelayCommand AddClassCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand<Class> EditClassCommand { get; private set; }
        public RelayCommand RemoveClassCommand { get; private set; }

        #endregion Commands

        public ObservableCollection<Class> Classes
        {
            get { return classes; }
            set { SetProperty(ref classes, value); }
        }

        public Class SelectedClass
        {
            get { return selectedClass; }
            set
            {
                SetProperty(ref selectedClass, value);
                LoadClassProfiles(selectedClass.Id);
            }
        }

        public ObservableCollection<Profile> ClassProfiles
        {
            get { return classProfiles; }
            set { SetProperty(ref classProfiles, value); }
        }

        public void LoadData()
        {
            Console.WriteLine("ClassesView load data.");
            //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //{
            //    return;
            //}
            allClasses = repo.GetClasses();
            Classes = new ObservableCollection<Class>(allClasses);
        }

        private void LoadClassProfiles(int classId)
        {
            Class @class = repo.LoadClassProfiles(classId);
            ClassProfiles = new ObservableCollection<Profile>(@class.Profiles);
        }

        private void OnAddClass()
        {
            AddClassRequested(new Class());
        }

        private void OnEditClass(Class @class)
        {
            EditClassRequested(@class);
        }
    }
}