using ATEK.AccessControl_2.Services;
using ATEK.Data.Migrations;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Profiles
{
    public class AddEditProfileViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private Profile editingProfile = null;
        private List<Class> allClasses;
        private SimpleEditableProfile profile;
        private ObservableCollection<Class> classes;
        private bool editMode;
        private string addEditProblem;

        public AddEditProfileViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;

            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave, CanSave);
        }

        //=====================================================================

        #region Commands

        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        #endregion Commands

        //=====================================================================

        #region Actions

        public event Action Done = delegate { };

        #endregion Actions

        //=====================================================================

        #region Methods

        private void OnCancel()
        {
            Done();
        }

        private void OnSave()
        {
            if (UpdateProfile(Profile, editingProfile))
            {
                if (EditMode)
                {
                    editingProfile.DateModified = DateTime.Today;
                    repo.UpdateProfile(editingProfile);
                    Done();
                }
                else
                {
                    editingProfile.DateCreated = DateTime.Today;
                    editingProfile.DateModified = DateTime.Today;
                    if (editingProfile.Status == null)
                    {
                        editingProfile.Status = "Active";
                    }
                    if (!repo.AddProfile(editingProfile))
                    {
                        AddEditProblem = "Cannot Save Profile";
                    }
                    else
                    {
                        Done();
                    }
                }
            }
        }

        private bool CanSave()
        {
            return !Profile.HasErrors;
        }

        public void SetProfile(Profile profile)
        {
            editingProfile = profile;
            if (Profile != null) Profile.ErrorsChanged -= RaiseCanExecuteChanged;
            Profile = new SimpleEditableProfile();
            Profile.ErrorsChanged += RaiseCanExecuteChanged;
            CopyProfile(profile, Profile);
        }

        private void RaiseCanExecuteChanged(object sender, EventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void CopyProfile(Profile source, SimpleEditableProfile target)
        {
            target.Id = source.Id;
            if (EditMode)
            {
                target.Pinno = source.Pinno;
                target.Adno = source.Adno;
                target.Name = source.Name;
                target.Gender = source.Gender;
                target.DateOfBirth = source.DateOfBirth;
                target.DateOfIssue = source.DateOfIssue;
                target.Email = source.Email;
                target.Address = source.Address;
                target.Phone = source.Phone;
                target.Status = source.Status;
                target.Image = source.Image;
                target.DateToLock = source.DateToLock;
                target.CheckDateToLock = source.CheckDateToLock;
                target.LicensePlate = source.LicensePlate;
                target.DateCreated = source.DateCreated;
                target.DateModified = source.DateModified;
                target.Class = source.Class;
                target.ClassId = source.ClassId;
            }
        }

        private bool UpdateProfile(SimpleEditableProfile source, Profile target)
        {
            target.Pinno = source.Pinno;
            target.Adno = source.Adno;
            target.Name = source.Name;
            target.Gender = source.Gender;
            target.DateOfBirth = source.DateOfBirth;
            target.DateOfIssue = source.DateOfIssue;
            target.Email = source.Email;
            target.Address = source.Address;
            target.Phone = source.Phone;
            target.Status = source.Status;
            target.Image = source.Image;
            target.DateToLock = source.DateToLock;
            target.CheckDateToLock = source.CheckDateToLock;
            target.LicensePlate = source.LicensePlate;
            target.DateCreated = source.DateCreated;
            target.DateModified = source.DateModified;
            target.Class = source.Class;
            target.ClassId = source.ClassId;
            return true;
        }

        public void LoadData()
        {
            LoadClasses();
        }

        private void LoadClasses()
        {
            allClasses = repo.GetClasses().ToList();
            Classes = new ObservableCollection<Class>(allClasses);
        }

        #endregion Methods

        //=====================================================================

        #region Properties

        public string AddEditProblem
        {
            get { return addEditProblem; }
            set { SetProperty(ref addEditProblem, value); }
        }

        public ObservableCollection<Class> Classes
        {
            get { return classes; }
            set { SetProperty(ref classes, value); }
        }

        public SimpleEditableProfile Profile
        {
            get { return profile; }
            set { SetProperty(ref profile, value); }
        }

        public bool EditMode
        {
            get { return editMode; }
            set { SetProperty(ref editMode, value); }
        }

        #endregion Properties

        //=====================================================================
    }
}