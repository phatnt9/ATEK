using ATEK.AccessControl_2.Services;
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
        private bool editMode;

        public AddEditClassViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave, CanSave);
        }

        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public event Action Done = delegate { };

        private Class editingClass = null;
        private SimpleEditableClass @class;

        private void OnCancel()
        {
            Done();
        }

        private void OnSave()
        {
            if (UpdateClass(Class, editingClass))
            {
                if (EditMode)
                    repo.UpdateClass(editingClass);
                else
                    repo.AddClass(editingClass);
                Done();
            }
        }

        private bool CanSave()
        {
            return !Class.HasErrors;
        }

        public void SetClass(Class @class)
        {
            editingClass = @class;
            if (Class != null) Class.ErrorsChanged -= RaiseCanExecuteChanged;
            Class = new SimpleEditableClass();
            Class.ErrorsChanged += RaiseCanExecuteChanged;
            CopyClass(@class, Class);
        }

        private void RaiseCanExecuteChanged(object sender, EventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void CopyClass(Class source, SimpleEditableClass target)
        {
            target.Id = source.Id;
            if (EditMode)
            {
                target.Name = source.Name;
            }
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

        public SimpleEditableClass Class { get { return @class; } set { SetProperty(ref @class, value); } }
        public bool EditMode { get { return editMode; } set { SetProperty(ref editMode, value); } }
    }
}