using ATEK.Core.Services;
using ATEK.Core.ViewModels;
using MvvmCross;
using MvvmCross.Logging;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            Mvx.IoCProvider.RegisterType<ILoginService, LoginService>();
            Mvx.IoCProvider.RegisterType<IMvxLog, Logger>();
            RegisterAppStart<MainViewModel>();
        }
    }
}