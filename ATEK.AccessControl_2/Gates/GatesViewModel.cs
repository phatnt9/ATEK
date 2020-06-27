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
        private string setProgress;
        private int setProgressValue;

        private int searchGateProfilesByClass;
        private string searchGateProfilesInput;
        private bool customActiveTimesChecked;
        private ObservableCollection<string> hoursList = new ObservableCollection<string>() { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
        private ObservableCollection<string> minutesList = new ObservableCollection<string>() { "00", "05", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "59" };
        private string first_ActiveTime_From_Hour;
        private string first_ActiveTime_From_Minute;
        private string first_ActiveTime_To_Hour;
        private string first_ActiveTime_To_Minute;
        private string second_ActiveTime_From_Hour;
        private string second_ActiveTime_From_Minute;
        private string second_ActiveTime_To_Hour;
        private string second_ActiveTime_To_Minute;
        private Profile selectedProfile;
        private BackgroundWorker setActiveTimesBackGroundWorker;
        private BackgroundWorker getTimeChecksBackGroundWorker;
        private bool isBackGroundWorkerBusy;

        public GatesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddGateCommand = new RelayCommand(OnAddGate);
            GetTimeChecksCommand = new RelayCommand<Gate>(OnGetTimeChecks);
            EditGateCommand = new RelayCommand<Gate>(OnEditGate);
            ManageGateCommand = new RelayCommand<Gate>(OnManageGate);
            RefreshGatesCommand = new RelayCommand(OnRefreshGates);
            ApplyActiveTimeCommand = new RelayCommand<object>(OnApplyActiveTime);
            StopApplyActiveTimeCommand = new RelayCommand(OnStopSetActiveTime);
            CardSweepCommand = new RelayCommand<Profile>(OnCardSweep);
        }

        //=====================================================================

        #region Commands

        public RelayCommand AddGateCommand { get; private set; }
        public RelayCommand<Gate> GetTimeChecksCommand { get; private set; }
        public RelayCommand<Gate> EditGateCommand { get; private set; }
        public RelayCommand<Gate> ManageGateCommand { get; private set; }
        public RelayCommand RefreshGatesCommand { get; private set; }
        public RelayCommand<object> ApplyActiveTimeCommand { get; private set; }
        public RelayCommand StopApplyActiveTimeCommand { get; private set; }
        public RelayCommand<Profile> CardSweepCommand { get; private set; }

        #endregion Commands

        //=====================================================================

        #region Actions

        public event Action<Gate> AddGateRequested = delegate { };

        public event Action<Gate> EditGateRequested = delegate { };

        public event Action<Gate> ManageGateRequested = delegate { };

        public event Action StartBackgroundProgress = delegate { };

        public event Action StopBackgroundProgress = delegate { };

        #endregion Actions

        //=====================================================================

        #region Properties

        public bool IsBackGroundWorkerBusy
        {
            get { return isBackGroundWorkerBusy; }
            set
            {
                SetProperty(ref isBackGroundWorkerBusy, value);
                if (isBackGroundWorkerBusy)
                {
                    StartBackgroundProgress();
                }
                else
                {
                    StopBackgroundProgress();
                }
            }
        }

        public string SetProgress
        {
            get { return setProgress; }
            set { SetProperty(ref setProgress, value); }
        }

        public int SetProgressValue
        {
            get { return setProgressValue; }
            set
            {
                SetProperty(ref setProgressValue, value);
                SetProgress = setProgressValue + "%";
            }
        }

        public string First_ActiveTime_From_Hour { get { return first_ActiveTime_From_Hour; } set { SetProperty(ref first_ActiveTime_From_Hour, value); } }
        public string First_ActiveTime_From_Minute { get { return first_ActiveTime_From_Minute; } set { SetProperty(ref first_ActiveTime_From_Minute, value); } }
        public string First_ActiveTime_To_Hour { get { return first_ActiveTime_To_Hour; } set { SetProperty(ref first_ActiveTime_To_Hour, value); } }
        public string First_ActiveTime_To_Minute { get { return first_ActiveTime_To_Minute; } set { SetProperty(ref first_ActiveTime_To_Minute, value); } }
        public string Second_ActiveTime_From_Hour { get { return second_ActiveTime_From_Hour; } set { SetProperty(ref second_ActiveTime_From_Hour, value); } }
        public string Second_ActiveTime_From_Minute { get { return second_ActiveTime_From_Minute; } set { SetProperty(ref second_ActiveTime_From_Minute, value); } }
        public string Second_ActiveTime_To_Hour { get { return second_ActiveTime_To_Hour; } set { SetProperty(ref second_ActiveTime_To_Hour, value); } }
        public string Second_ActiveTime_To_Minute { get { return second_ActiveTime_To_Minute; } set { SetProperty(ref second_ActiveTime_To_Minute, value); } }

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
                    Console.WriteLine($"Selected gate is: {selectedGate.Name}");
                    LoadGateProfiles(selectedGate.Id);
                }
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
                    foreach (var pg in selectedProfile.ProfileGates)
                    {
                        Console.WriteLine($"GateId: {pg.GateId}");
                        string rs = $"[{pg.ActiveTimes.Count}]Activetimes: ";
                        if (pg.ActiveTimes.Count == 2)
                        {
                            string temp = $"{pg.ActiveTimes[0].ToString()}-{pg.ActiveTimes[1].ToString()}";
                            Console.WriteLine(rs + temp);
                            Console.WriteLine("========================================");
                        }
                    }
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

        public bool CustomActiveTimesChecked
        {
            get { return customActiveTimesChecked; }
            set { SetProperty(ref customActiveTimesChecked, value); }
        }

        public ObservableCollection<string> HoursList
        {
            get { return hoursList; }
            set { SetProperty(ref hoursList, value); }
        }

        public ObservableCollection<string> MinutesList
        {
            get { return minutesList; }
            set { SetProperty(ref minutesList, value); }
        }

        #endregion Properties

        //=====================================================================

        #region Methods

        private void OnGetTimeChecks(Gate gate)
        {
            if (gate != null)
            {
                List<TimeCheck> gateListTimeChecks = repo.Firebase_GetTimeChecks(gate.FirebaseId);
                if (gateListTimeChecks != null)
                {
                    getTimeChecksBackGroundWorker = new BackgroundWorker();
                    getTimeChecksBackGroundWorker.WorkerSupportsCancellation = true;
                    getTimeChecksBackGroundWorker.WorkerReportsProgress = true;
                    getTimeChecksBackGroundWorker.DoWork += GetTimeChecksBackGroundWorker_DoWork;
                    getTimeChecksBackGroundWorker.RunWorkerCompleted += GetTimeChecksBackGroundWorker_RunWorkerCompleted;
                    getTimeChecksBackGroundWorker.ProgressChanged += GetTimeChecksBackGroundWorker_ProgressChanged;
                    getTimeChecksBackGroundWorker.Disposed += GetTimeChecksBackGroundWorker_Disposed;
                    List<object> args = new List<object>();
                    args.Add(gate.FirebaseId);
                    args.Add(gateListTimeChecks);
                    getTimeChecksBackGroundWorker.RunWorkerAsync(args);
                    IsBackGroundWorkerBusy = true;
                }
            }
        }

        private void GetTimeChecksBackGroundWorker_Disposed(object sender, EventArgs e)
        {
            IsBackGroundWorkerBusy = false;
        }

        private void GetTimeChecksBackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetProgressValue = e.ProgressPercentage;
        }

        private void GetTimeChecksBackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // check error, check cancel, then use result
            if (e.Error != null)
            {
                // handle the error
            }
            else if (e.Cancelled)
            {
                // handle cancellation
            }
            else
            {
            }
            // general cleanup code, runs when there was an error or not.
            SetProgressValue = 0;
            getTimeChecksBackGroundWorker.Dispose();
        }

        private void GetTimeChecksBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<object> args = (List<object>)e.Argument;
            var gateFirebaseId = (string)args[0];
            var gateListTimeChecks = (List<TimeCheck>)args[1];
            for (int i = 0; i < gateListTimeChecks.Count; i++)
            {
                if (repo.AddTimeCheck(gateListTimeChecks[i]))
                {
                    if (!repo.Firebase_RemoveTimeCheck(gateFirebaseId, gateListTimeChecks[i].FirebaseId))
                    {
                        MessageBox.Show("Error, Please check your internet");
                        return;
                    }
                }
                if (getTimeChecksBackGroundWorker.CancellationPending)
                {
                    return;
                }
                (sender as BackgroundWorker).ReportProgress((i * 100) / gateListTimeChecks.Count);
            }
        }

        private void OnCardSweep(Profile profile)
        {
            for (int i = 0; i < 1000; i++)
            {
                TimeCheck timeCheck = new TimeCheck()
                {
                    GateFirebaseId = selectedGate.FirebaseId,
                    Pinno = profile.Pinno,
                    Timecheck = DateTime.Now
                };
                repo.Firebase_AddTimeCheck(timeCheck);
            }
        }

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
            if (allClasses != null)
            {
                allClasses.Insert(0, new Class() { Id = 0, Name = "All" });
                Classes = new ObservableCollection<Class>(allClasses);
                FilterGateProfiles(searchGateProfilesInput, searchGateProfilesByClass);
            }
            else
            {
                Classes = new ObservableCollection<Class>();
            }
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
                        OnRemoveGate(gate);
                    }
                    else
                    {
                        Console.WriteLine($"This Gate {gate.FirebaseId} existed in firebase.");
                    }
                }
                allGates = repo.GetGates().ToList();
                Gates = new ObservableCollection<Gate>(allGates);
            }
            else
            {
                MessageBox.Show("Error, Please check your internet");
                Gates = new ObservableCollection<Gate>();
            }
        }

        private void LoadGateProfiles(int gateId)
        {
            allGateProfiles = repo.LoadProfilesOfGate(gateId).ToList();
            if (allGateProfiles != null)
            {
                GateProfiles = new ObservableCollection<Profile>(allGateProfiles);
                FilterGateProfiles(searchGateProfilesInput, searchGateProfilesByClass);
            }
            else
            {
                GateProfiles = new ObservableCollection<Profile>();
            }
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

        private void OnStopSetActiveTime()
        {
            if (setActiveTimesBackGroundWorker != null && setActiveTimesBackGroundWorker.WorkerSupportsCancellation)
            {
                setActiveTimesBackGroundWorker.CancelAsync();
            }
        }

        private void OnApplyActiveTime(object obj)
        {
            if (obj != null && SelectedGate != null)
            {
                System.Collections.IList items = (System.Collections.IList)obj;
                var collection = items.Cast<Profile>();
                if (collection.Count() > 0)
                {
                    setActiveTimesBackGroundWorker = new BackgroundWorker();
                    setActiveTimesBackGroundWorker.WorkerSupportsCancellation = true;
                    setActiveTimesBackGroundWorker.WorkerReportsProgress = true;
                    setActiveTimesBackGroundWorker.DoWork += SetActiveTimesBackGroundWorker_DoWork;
                    setActiveTimesBackGroundWorker.RunWorkerCompleted += SetActiveTimesBackGroundWorker_RunWorkerCompleted;
                    setActiveTimesBackGroundWorker.ProgressChanged += SetActiveTimesBackGroundWorker_ProgressChanged;
                    setActiveTimesBackGroundWorker.Disposed += SetActiveTimesBackGroundWorker_Disposed;
                    setActiveTimesBackGroundWorker.RunWorkerAsync(collection.ToList());
                    IsBackGroundWorkerBusy = true;
                }
            }
        }

        private void SetActiveTimesBackGroundWorker_Disposed(object sender, EventArgs e)
        {
            IsBackGroundWorkerBusy = false;
        }

        private void SetActiveTimesBackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetProgressValue = e.ProgressPercentage;
        }

        private void SetActiveTimesBackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // check error, check cancel, then use result
            if (e.Error != null)
            {
                // handle the error
            }
            else if (e.Cancelled)
            {
                // handle cancellation
            }
            else
            {
            }
            // general cleanup code, runs when there was an error or not.
            SetProgressValue = 0;
            setActiveTimesBackGroundWorker.Dispose();
        }

        private void SetActiveTimesBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var listProfiles = (List<Profile>)e.Argument;
            for (int i = 0; i < listProfiles.Count; i++)
            {
                var profileGate = listProfiles[i].ProfileGates.FirstOrDefault(pg => pg.GateId == SelectedGate.Id);
                if (profileGate != null)
                {
                    string activeTime = SetActiveTimeToProfileGate(profileGate);
                    if (activeTime != null)
                    {
                        if (!repo.Firebase_UpdateProfileGateActiveTime(activeTime, SelectedGate.FirebaseId, listProfiles[i].Pinno))
                        {
                            MessageBox.Show("Error, Please check your internet.");
                            return;
                        }
                    }
                }
                if (setActiveTimesBackGroundWorker.CancellationPending)
                {
                    return;
                }
                (sender as BackgroundWorker).ReportProgress((i * 100) / listProfiles.Count);
            }
        }

        private string SetActiveTimeToProfileGate(ProfileGate profileGate)
        {
            var listActiveTimes = GetActiveTime(profileGate.ProfileId, profileGate.GateId);
            if (listActiveTimes != null)
            {
                if (profileGate.ActiveTimes.Count <= 0)
                {
                    //New
                    bool result = true;
                    foreach (var activeTime in listActiveTimes)
                    {
                        if (!repo.AddActiveTime(activeTime))
                        {
                            result = false;
                            break;
                        }
                    }
                    if (!result)
                    {
                        return null;
                    }
                    else
                    {
                        return $"{listActiveTimes[0].ToString()}-{listActiveTimes[1].ToString()}";
                    }
                }
                else
                {
                    //Update
                    bool result = true;
                    for (int i = 0; i < 2; i++)
                    {
                        if (!repo.RemoveActiveTime(profileGate.ActiveTimes.First()))
                        {
                            result = false;
                            break;
                        }
                        if (!repo.AddActiveTime(listActiveTimes[i]))
                        {
                            result = false;
                            break;
                        }
                    }
                    if (!result)
                    {
                        return null;
                    }
                    else
                    {
                        return $"{listActiveTimes[0].ToString()}-{listActiveTimes[1].ToString()}";
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public List<ActiveTime> GetActiveTime(int profileGateProfileId, int profileGateGateId)
        {
            if (CustomActiveTimesChecked)
            {
                //New ActiveTimes
                if (!string.IsNullOrWhiteSpace(first_ActiveTime_From_Hour) &&
                    !string.IsNullOrWhiteSpace(first_ActiveTime_From_Minute) &&
                    !string.IsNullOrWhiteSpace(first_ActiveTime_To_Hour) &&
                    !string.IsNullOrWhiteSpace(first_ActiveTime_To_Minute) &&
                    !string.IsNullOrWhiteSpace(second_ActiveTime_From_Hour) &&
                    !string.IsNullOrWhiteSpace(second_ActiveTime_From_Minute) &&
                    !string.IsNullOrWhiteSpace(second_ActiveTime_To_Hour) &&
                    !string.IsNullOrWhiteSpace(second_ActiveTime_To_Minute)
                    )
                {
                    var firstActiveTime = new ActiveTime()
                    {
                        FromTime = $"{first_ActiveTime_From_Hour}:{first_ActiveTime_From_Minute}",
                        ToTime = $"{first_ActiveTime_To_Hour}:{first_ActiveTime_To_Minute}",
                        ProfileGateProfileId = profileGateProfileId,
                        ProfileGateGateId = profileGateGateId
                    };
                    var secondActiveTime = new ActiveTime()
                    {
                        FromTime = $"{second_ActiveTime_From_Hour}:{second_ActiveTime_From_Minute}",
                        ToTime = $"{second_ActiveTime_To_Hour}:{second_ActiveTime_To_Minute}",
                        ProfileGateProfileId = profileGateProfileId,
                        ProfileGateGateId = profileGateGateId
                    };

                    List<ActiveTime> result = new List<ActiveTime>();
                    result.Add(firstActiveTime);
                    result.Add(secondActiveTime);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //24/24
                var firstActiveTime = new ActiveTime()
                {
                    FromTime = "00:00",
                    ToTime = "23:59",
                    ProfileGateProfileId = profileGateProfileId,
                    ProfileGateGateId = profileGateGateId
                };
                var secondActiveTime = new ActiveTime()
                {
                    FromTime = "00:00",
                    ToTime = "23:59",
                    ProfileGateProfileId = profileGateProfileId,
                    ProfileGateGateId = profileGateGateId
                };

                List<ActiveTime> result = new List<ActiveTime>();
                result.Add(firstActiveTime);
                result.Add(secondActiveTime);
                return result;
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

        private async void OnRefreshGates()
        {
            await LoadGatesAsync();
        }

        private void OnRemoveGate(Gate gate)
        {
            List<Gate> deletes = new List<Gate>();
            deletes.Add(gate);
            if (repo.Firebase_RemoveGate(gate))
            {
                if (!repo.RemoveGates(deletes))
                {
                    repo.Firebase_AddGate(gate);
                }
                else
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Error, Please check your internet");
            }
        }

        #endregion Methods

        //=====================================================================
    }
}