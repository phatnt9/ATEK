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
        private List<Class> allClasses_Firebase;
        private ObservableCollection<Class> classes;
        private Class selectedClass;

        public ClassesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddClassCommand = new RelayCommand(OnAddClass);
            EditClassCommand = new RelayCommand<Class>(OnEditClass);
            RemoveClassCommand = new RelayCommand<Class>(OnRemoveClass);
        }

        #region Actions

        public event Action<Class> AddClassRequested = delegate { };

        public event Action<Class> EditClassRequested = delegate { };

        #endregion Actions

        #region Commands

        public RelayCommand AddClassCommand { get; private set; }
        public RelayCommand<Class> EditClassCommand { get; private set; }
        public RelayCommand<Class> RemoveClassCommand { get; private set; }

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
            }
        }

        public async void LoadData()
        {
            await LoadClassesAsync();
        }

        public async Task LoadClassesAsync()
        {
            var result = await repo.Firebase_GetClassesAsync();
            if (result != null)
            {
                allClasses = repo.GetClasses().ToList();
                allClasses_Firebase = result.ToList();
                foreach (var fbClass in allClasses_Firebase)
                {
                    if (!allClasses.Exists(g => g.Id == fbClass.Id))
                    {
                        Console.WriteLine($"This Class {fbClass.Name} doesn't existed in database.");
                        repo.AddClass(fbClass);
                    }
                    else
                    {
                        Console.WriteLine($"This Class {fbClass.Name} existed in database.");
                    }
                }
                allClasses = repo.GetClasses().ToList();
                foreach (var @class in allClasses)
                {
                    if (!allClasses_Firebase.Exists(g => g.Id == @class.Id))
                    {
                        Console.WriteLine($"This Class {@class.Name} doesn't existed in firebase.");
                        OnRemoveClass(@class);
                    }
                    else
                    {
                        Console.WriteLine($"This Class {@class.Name} existed in firebase.");
                    }
                }
                allClasses = repo.GetClasses().ToList();
                Classes = new ObservableCollection<Class>(allClasses);
            }
            else
            {
                MessageBox.Show("Error Load Classes");
                Classes = new ObservableCollection<Class>();
            }
        }

        private void OnAddClass()
        {
            AddClassRequested(new Class());
        }

        private void OnEditClass(Class @class)
        {
            EditClassRequested(@class);
        }

        private void OnRemoveClass(Class @class)
        {
            string message = "Delete class will remove all its associated profiles. Are you sure?";
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(message, "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                List<Class> deletes = new List<Class>();
                deletes.Add(@class);
                if (repo.Firebase_RemoveClass(@class))
                {
                    if (!repo.RemoveClasses(deletes))
                    {
                        repo.Firebase_AddClass(@class);
                    }
                    LoadData();
                }
                else
                {
                    System.Windows.MessageBox.Show("Error, Please check your internet");
                }
            }
        }
    }
}