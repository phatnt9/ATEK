using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Profiles
{
    public class AddEditProfileViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private Profile editingProfile = null;
        private SimpleEditableProfile profile;
        private bool editMode;

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
                    repo.UpdateProfile(editingProfile);
                else
                    repo.AddProfile(editingProfile);
                Done();
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
                target.Class = source.Class;
                target.ClassId = source.ClassId;
                target.Gender = source.Gender;
                target.DateOfBirth = source.DateOfBirth;
                target.Email = source.Email;
                target.Address = source.Address;
                target.Phone = source.Phone;
                target.Image = source.Image;
                target.DateToLock = source.DateToLock;
                target.CheckDateToLock = source.CheckDateToLock;
                target.CheckDateToLock = source.CheckDateToLock;
                target.LicensePlate = source.LicensePlate;
            }
        }

        private bool UpdateProfile(SimpleEditableProfile source, Profile target)
        {
            target.Pinno = source.Pinno;
            target.Adno = source.Adno;
            target.Name = source.Name;
            target.Class = source.Class;
            target.ClassId = source.ClassId;
            target.Gender = source.Gender;
            target.DateOfBirth = source.DateOfBirth;
            target.Email = source.Email;
            target.Address = source.Address;
            target.Phone = source.Phone;
            target.Image = source.Image;
            target.DateToLock = source.DateToLock;
            target.CheckDateToLock = source.CheckDateToLock;
            target.CheckDateToLock = source.CheckDateToLock;
            target.LicensePlate = source.LicensePlate;
            return true;
        }

        #endregion Methods

        //=====================================================================

        #region Properties

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