using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATEK.AccessControl_2.Gates
{
    public class ManageGateViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private Gate gate;
        private List<Class> allClasses;
        private List<Profile> allProfiles;
        private List<Profile> allGateProfiles;
        private ObservableCollection<Class> classes;
        private ObservableCollection<Profile> profiles;
        private ObservableCollection<Profile> gateProfiles;
        private BackgroundWorker selectBackGroundWorker;
        private BackgroundWorker removeBackGroundWorker;
        private bool isBackGroundWorkerBusy;

        private string selectProgress;
        private int selectProgressValue;
        private string removeProgress;
        private int removeProgressValue;
        private int searchProfilesByClass;
        private string searchProfilesInput;
        private int searchGateProfilesByClass;
        private string searchGateProfilesInput;

        public ManageGateViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            selectBackGroundWorker = new BackgroundWorker();
            removeBackGroundWorker = new BackgroundWorker();
            CancelCommand = new RelayCommand(OnCancel);
            SelectProfilesCommand = new RelayCommand<object>(OnSelectProfiles);
            DeleteGateProfilesCommand = new RelayCommand<object>(OnRemoveGateProfiles);
            StopSelectProfilesCommand = new RelayCommand(OnStopSelectProfiles);
            StopDeleteGateProfilesCommand = new RelayCommand(OnStopRemoveGateProfiles);
        }

        //=====================================================================

        #region Commands

        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand<object> SelectProfilesCommand { get; private set; }
        public RelayCommand StopSelectProfilesCommand { get; private set; }
        public RelayCommand<object> DeleteGateProfilesCommand { get; private set; }
        public RelayCommand StopDeleteGateProfilesCommand { get; private set; }

        #endregion Commands

        //=====================================================================

        #region Actions

        public event Action Done = delegate { };

        public event Action StartBackgroundProgress = delegate { };

        public event Action StopBackgroundProgress = delegate { };

        #endregion Actions

        //=====================================================================

        #region Properties

        public Gate Gate
        {
            get { return gate; }
            set { SetProperty(ref gate, value); }
        }

        public ObservableCollection<Class> Classes
        {
            get { return classes; }
            set { SetProperty(ref classes, value); }
        }

        public ObservableCollection<Profile> Profiles
        {
            get { return profiles; }
            set { SetProperty(ref profiles, value); }
        }

        public ObservableCollection<Profile> GateProfiles
        {
            get { return gateProfiles; }
            set { SetProperty(ref gateProfiles, value); }
        }

        public string SelectProgress
        {
            get { return selectProgress; }
            set { SetProperty(ref selectProgress, value); }
        }

        public string RemoveProgress
        {
            get { return removeProgress; }
            set { SetProperty(ref removeProgress, value); }
        }

        public int SelectProgressValue
        {
            get { return selectProgressValue; }
            set
            {
                SetProperty(ref selectProgressValue, value);
                SelectProgress = selectProgressValue + "%";
            }
        }

        public int RemoveProgressValue
        {
            get { return removeProgressValue; }
            set
            {
                SetProperty(ref removeProgressValue, value);
                RemoveProgress = removeProgressValue + "%";
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

        public string SearchProfilesInput
        {
            get { return searchProfilesInput; }
            set
            {
                SetProperty(ref searchProfilesInput, value);
                FilterProfiles(searchProfilesInput, searchProfilesByClass);
            }
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

        #endregion Properties

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

        private void FilterGateProfiles(string searchInput, int classId)
        {
            if (allGateProfiles != null)
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

        private void OnCancel()
        {
            Done();
        }

        public void SetGate(Gate gate)
        {
            Gate = gate;
        }

        public void LoadData()
        {
            LoadClasses();
            LoadProfiles();
            LoadGateProfiles();
        }

        private void LoadClasses()
        {
            allClasses = repo.GetClasses().ToList();
            allClasses.Insert(0, new Class() { Id = 0, Name = "All" });
            Classes = new ObservableCollection<Class>(allClasses);
            FilterProfiles(searchProfilesInput, searchProfilesByClass);
            FilterGateProfiles(searchGateProfilesInput, searchGateProfilesByClass);
        }

        public void LoadProfiles()
        {
            allProfiles = repo.GetProfiles().ToList();
            Profiles = new ObservableCollection<Profile>(allProfiles);
        }

        public void LoadGateProfiles()
        {
            allGateProfiles = repo.LoadProfilesOfGate(gate.Id).ToList();
            GateProfiles = new ObservableCollection<Profile>(allGateProfiles);
            FilterGateProfiles(searchGateProfilesInput, searchGateProfilesByClass);
        }

        private void OnStopSelectProfiles()
        {
            if (selectBackGroundWorker.WorkerSupportsCancellation)
            {
                selectBackGroundWorker.CancelAsync();
            }
        }

        private void OnStopRemoveGateProfiles()
        {
            if (removeBackGroundWorker.WorkerSupportsCancellation)
            {
                removeBackGroundWorker.CancelAsync();
            }
        }

        private void OnSelectProfiles(object obj)
        {
            if (obj != null)
            {
                List<Profile> list = new List<Profile>();
                System.Collections.IList items = (System.Collections.IList)obj;
                var collection = items.Cast<Profile>();
                if (collection.Count() > 0)
                {
                    foreach (var item in collection)
                    {
                        if (!gate.ProfileGates.Exists(g => (g.ProfileId == item.Id)))
                        {
                            list.Add(item);
                        }
                    }
                    if (list.Count() > 0)
                    {
                        selectBackGroundWorker = new BackgroundWorker();
                        selectBackGroundWorker.WorkerSupportsCancellation = true;
                        selectBackGroundWorker.WorkerReportsProgress = true;
                        selectBackGroundWorker.DoWork += SelectBackGroundWorker_DoWork;
                        selectBackGroundWorker.RunWorkerCompleted += SelectBackGroundWorker_RunWorkerCompleted;
                        selectBackGroundWorker.ProgressChanged += SelectBackGroundWorker_ProgressChanged;
                        selectBackGroundWorker.Disposed += SelectBackGroundWorker_Disposed;
                        selectBackGroundWorker.RunWorkerAsync(list);
                        IsBackGroundWorkerBusy = true;
                    }
                }
            }
        }

        private void SelectBackGroundWorker_Disposed(object sender, EventArgs e)
        {
            IsBackGroundWorkerBusy = false;
        }

        private void SelectBackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SelectProgressValue = e.ProgressPercentage;
        }

        private void SelectBackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            SelectProgressValue = 0;
            selectBackGroundWorker.Dispose();
            LoadGateProfiles();
        }

        private void SelectBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Profile> listProfiles = (List<Profile>)e.Argument;
            for (int i = 0; i < listProfiles.Count; i++)
            {
                //if (repo.Firebase_AddProfileGateAsync(listProfiles[i], gate).Result)
                if (repo.Firebase_AddProfileGate(listProfiles[i], gate))
                {
                    if (!repo.AddProfileGate(listProfiles[i], gate))
                    {
                        MessageBox.Show("Select Profile khong thanh cong.");
                        repo.Firebase_RemoveProfileGateAsync(listProfiles[i], gate);
                    }
                }
                else
                {
                    MessageBox.Show("Khong add len firebase duoc.");
                    return;
                }

                if (selectBackGroundWorker.CancellationPending)
                {
                    return;
                }
                (sender as BackgroundWorker).ReportProgress((i * 100) / listProfiles.Count);
            }
        }

        private void OnRemoveGateProfiles(object obj)
        {
            if (obj != null)
            {
                List<Profile> list = new List<Profile>();
                System.Collections.IList items = (System.Collections.IList)obj;
                var collection = items.Cast<Profile>();
                if (collection.Count() > 0)
                {
                    foreach (var item in collection)
                    {
                        list.Add(item);
                    }
                    if (list.Count() > 0)
                    {
                        removeBackGroundWorker = new BackgroundWorker();
                        removeBackGroundWorker.WorkerSupportsCancellation = true;
                        removeBackGroundWorker.WorkerReportsProgress = true;
                        removeBackGroundWorker.DoWork += RemoveBackGroundWorker_DoWork;
                        removeBackGroundWorker.RunWorkerCompleted += RemoveBackGroundWorker_RunWorkerCompleted;
                        removeBackGroundWorker.ProgressChanged += RemoveBackGroundWorker_ProgressChanged;
                        removeBackGroundWorker.Disposed += RemoveBackGroundWorker_Disposed;
                        removeBackGroundWorker.RunWorkerAsync(list);
                        IsBackGroundWorkerBusy = true;
                    }
                    else
                    {
                        Console.WriteLine("Co Delete ma bi trung het roi");
                    }
                }
            }
        }

        private void RemoveBackGroundWorker_Disposed(object sender, EventArgs e)
        {
            IsBackGroundWorkerBusy = false;
        }

        private void RemoveBackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            RemoveProgressValue = e.ProgressPercentage;
        }

        private void RemoveBackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            RemoveProgressValue = 0;
            removeBackGroundWorker.Dispose();
            LoadGateProfiles();
        }

        private void RemoveBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Profile> listProfiles = (List<Profile>)e.Argument;
            for (int i = 0; i < listProfiles.Count; i++)
            {
                //if (repo.Firebase_DeleteProfileGateAsync(listProfiles[i], gate).Result)
                if (repo.Firebase_RemoveProfileGate(listProfiles[i], gate))
                {
                    if (!repo.RemoveProfileGate(listProfiles[i], gate))
                    {
                        MessageBox.Show("Remove Profile khong thanh cong.");
                        repo.Firebase_AddProfileGateAsync(listProfiles[i], gate);
                    }
                }
                else
                {
                    MessageBox.Show("Khong delete len firebase duoc.");
                    return;
                }
                if (removeBackGroundWorker.CancellationPending)
                {
                    return;
                }
                (sender as BackgroundWorker).ReportProgress((i * 100) / listProfiles.Count);
            }
        }

        #endregion Methods

        //=====================================================================
    }
}