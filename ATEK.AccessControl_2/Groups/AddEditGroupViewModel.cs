﻿using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Groups
{
    public class AddEditGroupViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private bool editMode;

        public AddEditGroupViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave, CanSave);
        }

        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public event Action Done = delegate { };

        private Group editingGroup = null;
        private SimpleEditableGroup group;

        private void OnCancel()
        {
            Done();
        }

        private void OnSave()
        {
            if (UpdateGroup(Group, editingGroup))
            {
                if (EditMode)
                    repo.UpdateGroup(editingGroup);
                else
                    repo.AddGroup(editingGroup);
                Done();
            }
        }

        private bool CanSave()
        {
            return !Group.HasErrors;
        }

        public void SetGroup(Group group)
        {
            editingGroup = group;
            if (Group != null) Group.ErrorsChanged -= RaiseCanExecuteChanged;
            Group = new SimpleEditableGroup();
            Group.ErrorsChanged += RaiseCanExecuteChanged;
            CopyGroup(group, Group);
        }

        private void RaiseCanExecuteChanged(object sender, EventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void CopyGroup(Group source, SimpleEditableGroup target)
        {
            target.Id = source.Id;
            if (EditMode)
            {
                target.Name = source.Name;
            }
        }

        private bool UpdateGroup(SimpleEditableGroup source, Group target)
        {
            if (string.IsNullOrEmpty(source.Name))
                return false;
            else
            {
                target.Name = source.Name;
                return true;
            }
        }

        public SimpleEditableGroup Group { get { return group; } set { SetProperty(ref group, value); } }
        public bool EditMode { get { return editMode; } set { SetProperty(ref editMode, value); } }
    }
}