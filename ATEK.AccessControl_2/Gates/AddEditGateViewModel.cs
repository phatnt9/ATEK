using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Gates
{
    public class AddEditGateViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private SimpleEditableGate gate;
        private Gate editingGate = null;
        private bool editMode;

        public AddEditGateViewModel(IAccessControlRepository repo)
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

        public void SetGate(Gate gate)
        {
            editingGate = gate;
            if (Gate != null) Gate.ErrorsChanged -= RaiseCanExecuteChanged;
            Gate = new SimpleEditableGate();
            Gate.ErrorsChanged += RaiseCanExecuteChanged;
            CopyGate(gate, Gate);
        }

        private bool CanSave()
        {
            return (!Gate.HasErrors) && (!string.IsNullOrWhiteSpace(Gate.Name));
        }

        private void CopyGate(Gate source, SimpleEditableGate target)
        {
            target.Id = source.Id;
            if (EditMode)
            {
                target.Name = source.Name;
                target.Status = source.Status;
                target.Note = source.Note;
            }
        }

        private void OnCancel()
        {
            Done();
        }

        private void OnSave()
        {
            if (UpdateGate(Gate, editingGate))
            {
                if (EditMode)
                {
                    if (repo.Firebase_UpdateGate(editingGate))
                    {
                        if (!repo.UpdateGate(editingGate))
                        {
                            repo.Firebase_AddGate(editingGate);
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Error, Please check your internet");
                    }
                }
                else
                {
                    if (repo.AddGate(editingGate))
                    {
                        if (!repo.Firebase_AddGate(editingGate))
                        {
                            System.Windows.MessageBox.Show("Error, Please check your internet");
                            var deletes = new List<Gate>();
                            deletes.Add(editingGate);
                            repo.RemoveGates(deletes);
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

        private bool UpdateGate(SimpleEditableGate source, Gate target)
        {
            if (string.IsNullOrEmpty(source.Name))
                return false;
            else
            {
                target.Name = source.Name;
                target.Status = source.Status;
                target.Note = source.Note;
                return true;
            }
        }

        #endregion Methods

        //=====================================================================

        #region Properties

        public bool EditMode
        {
            get { return editMode; }
            set { SetProperty(ref editMode, value); }
        }

        public SimpleEditableGate Gate
        {
            get { return gate; }
            set { SetProperty(ref gate, value); }
        }

        #endregion Properties

        //=====================================================================
    }
}