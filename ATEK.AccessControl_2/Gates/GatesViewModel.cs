using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATEK.AccessControl_2.Gates
{
    public class GatesViewModel : ValidatableBindableBase
    {
        private readonly IAccessControlRepository repo;
        private Gate selectedGate;
        private List<Gate> allGates;
        private List<Gate> allGates_Firebase;
        private List<Class> allClasses;
        private List<Profile> allGateProfiles;
        private ObservableCollection<Gate> gates;
        private ObservableCollection<Class> classes;
        private ObservableCollection<Profile> gateProfiles;

        private int searchGateProfilesByClass;
        private string searchGateProfilesInput;

        public GatesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddGateCommand = new RelayCommand(OnAddGate);
            EditGateCommand = new RelayCommand<Gate>(OnEditGate);
            ManageGateCommand = new RelayCommand<Gate>(OnManageGate);
            RemoveGateCommand = new RelayCommand<Gate>(OnRemoveGate);
        }

        //=====================================================================

        #region Commands

        public RelayCommand AddGateCommand { get; private set; }
        public RelayCommand<Gate> EditGateCommand { get; private set; }
        public RelayCommand<Gate> ManageGateCommand { get; private set; }
        public RelayCommand<Gate> RemoveGateCommand { get; private set; }

        #endregion Commands

        //=====================================================================

        #region Actions

        public event Action<Gate> AddGateRequested = delegate { };

        public event Action<Gate> EditGateRequested = delegate { };

        public event Action<Gate> ManageGateRequested = delegate { };

        #endregion Actions

        //=====================================================================

        #region Properties

        public ObservableCollection<Gate> Gates
        {
            get { return gates; }
            set { SetProperty(ref gates, value); }
        }

        public Gate SelectedGate
        {
            get { return selectedGate; }
            set
            {
                SetProperty(ref selectedGate, value);
                if (selectedGate != null)
                {
                    LoadGateProfiles(selectedGate.Id);
                }
            }
        }

        public ObservableCollection<Profile> GateProfiles
        {
            get { return gateProfiles; }
            set { SetProperty(ref gateProfiles, value); }
        }

        public int SearchGateProfilesByClass
        {
            get { return searchGateProfilesByClass; }
            set
            {
                SetProperty(ref searchGateProfilesByClass, value);
                FilterGateProfiles(searchGateProfilesInput, searchGateProfilesByClass);
            }
        }

        public string SearchGateProfilesInput
        {
            get { return searchGateProfilesInput; }
            set
            {
                SetProperty(ref searchGateProfilesInput, value);
                FilterGateProfiles(searchGateProfilesInput, searchGateProfilesByClass);
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

        public async void LoadData()
        {
            LoadClasses();
            await LoadGatesAsync();
            if (selectedGate != null)
            {
                LoadGateProfiles(selectedGate.Id);
            }
        }

        private void LoadClasses()
        {
            allClasses = repo.GetClasses().ToList();
            allClasses.Insert(0, new Class() { Id = 0, Name = "All" });
            Classes = new ObservableCollection<Class>(allClasses);
            FilterGateProfiles(searchGateProfilesInput, searchGateProfilesByClass);
        }

        private async Task LoadGatesAsync()
        {
            var result = await repo.Firebase_GetGatesAsync();
            if (result != null)
            {
                allGates = repo.GetGates().ToList();
                allGates_Firebase = result.ToList();
                foreach (var fbGate in allGates_Firebase)
                {
                    if (!allGates.Exists(g => g.FirebaseId == fbGate.FirebaseId))
                    {
                        Console.WriteLine($"This Gate {fbGate.FirebaseId} doesn't existed in database.");
                        fbGate.Status = "Online";
                        repo.AddGate(fbGate);
                    }
                    else
                    {
                        Console.WriteLine($"This Gate {fbGate.FirebaseId} existed in database.");
                    }
                }
                allGates = repo.GetGates().ToList();
                foreach (var gate in allGates)
                {
                    if (!allGates_Firebase.Exists(g => g.FirebaseId == gate.FirebaseId))
                    {
                        Console.WriteLine($"This Gate {gate.FirebaseId} doesn't existed in firebase.");
                        gate.Status = "Offline";
                        repo.UpdateGate(gate);
                    }
                    else
                    {
                        Console.WriteLine($"This Gate {gate.FirebaseId} existed in firebase.");
                        gate.Status = "Online";
                        repo.UpdateGate(gate);
                    }
                }
                allGates = repo.GetGates().ToList();
                Gates = new ObservableCollection<Gate>(allGates);
            }
            else
            {
                MessageBox.Show("Error Load Gates");
                Gates = new ObservableCollection<Gate>();
            }
        }

        private void LoadGateProfiles(int gateId)
        {
            allGateProfiles = repo.LoadProfilesOfGate(gateId).ToList();
            GateProfiles = new ObservableCollection<Profile>(allGateProfiles);
            FilterGateProfiles(searchGateProfilesInput, searchGateProfilesByClass);
        }

        private void FilterGateProfiles(string searchInput, int classId)
        {
            if (allGateProfiles != null)
            {
                if (SelectedGate != null && GateProfiles != null)
                {
                    if (string.IsNullOrWhiteSpace(searchInput))
                    {
                        if (classId == 0)
                        {
                            GateProfiles = new ObservableCollection<Profile>(allGateProfiles);
                            return;
                        }
                        else
                        {
                            GateProfiles = new ObservableCollection<Profile>(
                          allGateProfiles.Where(c =>
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
                            GateProfiles = new ObservableCollection<Profile>(
                           allGateProfiles.Where(c =>
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
                            GateProfiles = new ObservableCollection<Profile>(
                           allGateProfiles.Where(c =>
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

        private void OnAddGate()
        {
            AddGateRequested(new Gate());
        }

        private void OnEditGate(Gate gate)
        {
            EditGateRequested(gate);
        }

        private void OnManageGate(Gate gate)
        {
            ManageGateRequested(gate);
        }

        private void OnRemoveGate(Gate gate)
        {
            List<Gate> deletes = new List<Gate>();
            deletes.Add(gate);
            repo.RemoveGates(deletes);
            LoadData();
        }

        #endregion Methods

        //=====================================================================
    }
}