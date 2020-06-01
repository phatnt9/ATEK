using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Groups
{
    public class ManageGroupViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private Group group;
        private List<Profile> allProfiles;
        private List<Profile> allGroupProfiles;
        private ObservableCollection<Profile> profiles;
        private ObservableCollection<Profile> groupProfiles;
        private BackgroundWorker selectBackGroundWorker;
        private BackgroundWorker removeBackGroundWorker;
        private bool isBackGroundWorkerBusy;

        private string selectProgress;
        private int selectProgressValue;
        private string removeProgress;
        private int removeProgressValue;

        public ManageGroupViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            selectBackGroundWorker = new BackgroundWorker();
            removeBackGroundWorker = new BackgroundWorker();
            CancelCommand = new RelayCommand(OnCancel);
            SelectProfilesCommand = new RelayCommand<object>(OnSelectProfiles);
            DeleteGroupProfilesCommand = new RelayCommand<object>(OnDeleteGroupProfiles);
            StopSelectProfilesCommand = new RelayCommand(OnStopSelectProfiles);
            StopDeleteGroupProfilesCommand = new RelayCommand(OnStopDeleteGroupProfiles);
        }

        #region Properties

        public Group Group { get { return group; } set { SetProperty(ref group, value); } }
        public ObservableCollection<Profile> Profiles { get { return profiles; } set { SetProperty(ref profiles, value); } }
        public ObservableCollection<Profile> GroupProfiles { get { return groupProfiles; } set { SetProperty(ref groupProfiles, value); } }

        public string SelectProgress { get { return selectProgress; } set { SetProperty(ref selectProgress, value); } }
        public string RemoveProgress { get { return removeProgress; } set { SetProperty(ref removeProgress, value); } }

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

        public bool IsBackGroundWorkerBusy
        {
            get { return isBackGroundWorkerBusy; }
            set
            {
                SetProperty(ref isBackGroundWorkerBusy, value);
            }
        }

        #endregion Properties

        #region Commands

        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand<object> SelectProfilesCommand { get; private set; }
        public RelayCommand StopSelectProfilesCommand { get; private set; }
        public RelayCommand<object> DeleteGroupProfilesCommand { get; private set; }
        public RelayCommand StopDeleteGroupProfilesCommand { get; private set; }

        public event Action Done = delegate { };

        #endregion Commands

        #region Methods

        private void OnCancel()
        {
            Done();
        }

        public void SetGroup(Group group)
        {
            //Group = group;
            Group = repo.GetGroupWithAllRelatedData(group.Id);
        }

        public void LoadData()
        {
            LoadProfiles();
            LoadGroupProfiles();
        }

        public void LoadProfiles()
        {
            allProfiles = repo.GetProfiles().ToList();
            Profiles = new ObservableCollection<Profile>(allProfiles);
        }

        public void LoadGroupProfiles()
        {
            allGroupProfiles = repo.LoadGroupProfiles(group.Id).ToList();
            GroupProfiles = new ObservableCollection<Profile>(allGroupProfiles);
        }

        private void OnStopDeleteGroupProfiles()
        {
            if (removeBackGroundWorker.WorkerSupportsCancellation)
            {
                removeBackGroundWorker.CancelAsync();
            }
        }

        private void OnStopSelectProfiles()
        {
            if (selectBackGroundWorker.WorkerSupportsCancellation)
            {
                selectBackGroundWorker.CancelAsync();
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
                        if (!Group.ProfileGroups.Exists(g => (g.ProfileId == item.Id)))
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
            LoadGroupProfiles();
        }

        private void SelectBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Profile> listProfiles = (List<Profile>)e.Argument;
            for (int i = 0; i < listProfiles.Count; i++)
            {
                if (!repo.AddProfileToGroup(group, listProfiles[i]))
                {
                    Console.WriteLine("Select Profile khong thanh cong.");
                }
                if (selectBackGroundWorker.CancellationPending)
                {
                    return;
                }
                (sender as BackgroundWorker).ReportProgress((i * 100) / listProfiles.Count);
            }
        }

        private async void OnSelectProfilesAsync(object obj)
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
                        var groupProfile = new ProfileGroup() { GroupId = group.Id, ProfileId = item.Id };
                        if (!Group.ProfileGroups.Exists(g => (g.ProfileId == item.Id)))
                        {
                            list.Add(item);
                        }
                    }
                    if (list.Count() > 0)
                    {
                        await repo.AddProfilesToGroupAsync(group, list);
                        LoadData();
                    }
                    else
                    {
                        Console.WriteLine("Co Select ma bi trung het roi");
                    }
                }
            }
        }

        private void OnDeleteGroupProfiles(object obj)
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
            LoadGroupProfiles();
        }

        private void RemoveBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Profile> listProfiles = (List<Profile>)e.Argument;
            for (int i = 0; i < listProfiles.Count; i++)
            {
                if (!repo.RemoveProfileFromGroup(group, listProfiles[i]))
                {
                    Console.WriteLine("Remove Profile khong thanh cong.");
                }
                if (removeBackGroundWorker.CancellationPending)
                {
                    return;
                }
                (sender as BackgroundWorker).ReportProgress((i * 100) / listProfiles.Count);
            }
        }

        private async void OnDeleteGroupProfilesAsync(object obj)
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
                        await repo.RemoveProfilesFromGroupAsync(group, list);
                        LoadData();
                    }
                    else
                    {
                        Console.WriteLine("Co Delete ma bi trung het roi");
                    }
                }
            }
        }

        #endregion Methods
    }
}