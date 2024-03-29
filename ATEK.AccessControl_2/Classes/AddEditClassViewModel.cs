﻿using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Classes
{
    public class AddEditClassViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private SimpleEditableClass @class;
        private Class editingClass = null;
        private bool editMode;

        public AddEditClassViewModel(IAccessControlRepository repo)
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

        public void SetClass(Class @class)
        {
            editingClass = @class;
            if (Class != null) Class.ErrorsChanged -= RaiseCanExecuteChanged;
            Class = new SimpleEditableClass();
            Class.ErrorsChanged += RaiseCanExecuteChanged;
            CopyClass(@class, Class);
        }

        private bool CanSave()
        {
            return (!Class.HasErrors) && (!string.IsNullOrWhiteSpace(Class.Name));
        }

        private void CopyClass(Class source, SimpleEditableClass target)
        {
            target.Id = source.Id;
            if (EditMode)
            {
                target.Name = source.Name;
            }
        }

        private void OnCancel()
        {
            Done();
        }

        private void OnSave()
        {
            if (UpdateClass(Class, editingClass))
            {
                if (EditMode)
                {
                    if (repo.Firebase_UpdateClass(editingClass))
                    {
                        if (!repo.UpdateClass(editingClass))
                        {
                            repo.Firebase_AddClass(editingClass);
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Error, Please check your internet.");
                    }
                }
                else
                {
                    if (repo.AddClass(editingClass))
                    {
                        if (!repo.Firebase_AddClass(editingClass))
                        {
                            System.Windows.MessageBox.Show("Error, Please check your internet.");
                            var deletes = new List<Class>();
                            deletes.Add(editingClass);
                            repo.RemoveClasses(deletes);
                        }
                    }
                }
                Done();
            }
        }

        private void RaiseCanExecuteChanged(object sender, EventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private bool UpdateClass(SimpleEditableClass source, Class target)
        {
            if (string.IsNullOrEmpty(source.Name))
                return false;
            else
            {
                target.Name = source.Name;
                return true;
            }
        }

        #endregion Methods

        //=====================================================================

        #region Properties

        public SimpleEditableClass Class { get { return @class; } set { SetProperty(ref @class, value); } }
        public bool EditMode { get { return editMode; } set { SetProperty(ref editMode, value); } }

        #endregion Properties

        //=====================================================================
    }
}