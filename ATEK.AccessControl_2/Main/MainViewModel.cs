using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATEK.AccessControl_2.Profiles;

namespace ATEK.AccessControl_2.Main
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            ProfilesViewModel = new ProfilesViewModel();
        }

        public object ProfilesViewModel { get; set; }
    }
}