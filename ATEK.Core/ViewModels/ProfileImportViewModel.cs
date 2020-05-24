using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ATEK.Core.ViewModels
{
    public class ProfileImportViewModel : MvxViewModel
    {
        private readonly IMvxLog logger;
        private readonly IMvxNavigationService navigationService;

        public ProfileImportViewModel(IMvxLog logger, IMvxNavigationService navigationService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
        }
    }
}