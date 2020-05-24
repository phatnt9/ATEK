using ATEK.Core.Services;
using MvvmCross.Binding;
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
    public class LoginViewModel : MvxViewModel<string>
    {
        private readonly ILoginService loginService;
        private readonly IMvxLog logger;
        private readonly IMvxNavigationService navigationService;

        public MvxCommand LoginCommand { get; set; }

        public LoginViewModel(ILoginService loginService, IMvxLog logger, IMvxNavigationService navigationService)
        {
            this.loginService = loginService;
            this.logger = logger;
            this.navigationService = navigationService;
            LoginCommand = new MvxCommand(() => Login());
        }

        public override async Task Initialize()
        {
            await base.Initialize();
        }

        public async void Login()
        {
            if (loginService.Login("", "") == true)
            {
                var result = await navigationService.Navigate<MainViewModel>();
            }
        }

        public override void Prepare(string parameter)
        {
            Console.WriteLine("Parameter: " + parameter);
        }
    }
}