using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.Core.ViewModels
{
    public class GatesViewModel : MvxViewModel
    {
        private readonly IMvxLog logger;

        public GatesViewModel(IMvxLog logger)
        {
            this.logger = logger;
        }

        public override async Task Initialize()
        {
            await base.Initialize();
        }
    }
}