using ATEK.Core.Services;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATEK.Core.ViewModels
{
    public class MainViewModel : MvxViewModel<string>
    {
        private readonly IMvxLog logger;
        private readonly IMvxNavigationService navigationService;

        public ProfilesViewModel ProfilesViewModel { get; set; }

        public MainViewModel(IMvxLog logger, IMvxNavigationService navigationService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            ProfilesViewModel = new ProfilesViewModel(logger, navigationService);
        }

        public override async Task Initialize()
        {
            await base.Initialize();
        }

        public override void Prepare(string parameter)
        {
            Console.WriteLine("Parameter: " + parameter);
        }
    }
}