using ATEK.Data.Contexts;
using ATEK.Domain.Models;
using Microsoft.EntityFrameworkCore;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ATEK.Core.ViewModels
{
    public class ProfilesViewModel : MvxViewModel
    {
        private readonly IMvxLog logger;
        private readonly IMvxNavigationService navigationService;
        private AccessControlContext _context;

        public ProfilesViewModel(IMvxLog logger, IMvxNavigationService navigationService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            _context = new AccessControlContext();
        }

        #region Commands

        private MvxCommand addCommand;
        public MvxCommand AddCommand => (addCommand = addCommand ?? new MvxCommand(() => Add()));

        private MvxCommand editCommand;
        public MvxCommand EditCommand => (editCommand = editCommand ?? new MvxCommand(() => Edit()));

        private MvxCommand removeCommand;
        public MvxCommand RemoveCommand => (removeCommand = removeCommand ?? new MvxCommand(() => Remove()));

        private MvxCommand refreshCommand;
        public MvxCommand RefreshCommand => (refreshCommand = refreshCommand ?? new MvxCommand(() => Refresh()));

        private MvxCommand importCommand;
        public MvxCommand ImportCommand => (importCommand = importCommand ?? new MvxCommand(() => Import()));

        #endregion Commands

        #region Methods

        public async void LoadData()
        {
            if (!IsDoingSthBackGround)
            {
                IsDoingSthBackGround = true;
                Profiles = new ObservableCollection<Profile>(await _context.Profiles.ToListAsync());
                IsDoingSthBackGround = false;
            }
        }

        public void Add()
        {
            Console.WriteLine("Add Profile");
        }

        public void Edit()
        {
            Console.WriteLine("Edit Profile");
        }

        public void Remove()
        {
            Console.WriteLine("Remove Profile");
        }

        public void Refresh()
        {
            Console.WriteLine("Refresh Profile");
        }

        public async void Import()
        {
            Console.WriteLine("Import Profile");
            await navigationService.Navigate<ProfileImportViewModel>();
        }

        #endregion Methods

        #region Properties

        private bool isDoingSthBackGround;

        public bool IsDoingSthBackGround
        {
            get => isDoingSthBackGround;
            set => SetProperty(ref isDoingSthBackGround, value);
        }

        private ObservableCollection<Profile> profiles;

        public ObservableCollection<Profile> Profiles
        {
            get => profiles;
            set => SetProperty(ref profiles, value);
        }

        #endregion Properties
    }
}