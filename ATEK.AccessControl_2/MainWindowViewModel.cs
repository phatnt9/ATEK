using ATEK.AccessControl_2.Login;
using ATEK.AccessControl_2.Main;
using System;
using System.Collections.Generic;
using Unity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2
{
    public class MainWindowViewModel : BindableBase
    {
        private BindableBase currentViewModel;
        private LoginViewModel loginViewModel = new LoginViewModel();
        private MainViewModel mainViewModel;

        public MainWindowViewModel()
        {
            MainViewModel = ContainerHelper.Container.Resolve<MainViewModel>();
            NavLoginCommand = new RelayCommand<string>(OnNavLogin);
            NavMainCommand = new RelayCommand<string>(OnNavMain);
        }

        public RelayCommand<string> NavLoginCommand { get; private set; }
        public RelayCommand<string> NavMainCommand { get; private set; }

        public void LoadData()
        {
            //CurrentViewModel = loginViewModel;
            CurrentViewModel = MainViewModel;
        }

        private void OnNavMain(string obj)
        {
            if (CurrentViewModel.GetType() != typeof(MainViewModel))
            {
                CurrentViewModel = MainViewModel;
            }
        }

        private void OnNavLogin(string obj)
        {
            if (CurrentViewModel.GetType() != typeof(LoginViewModel))
            {
                CurrentViewModel = loginViewModel;
            }
        }

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set { SetProperty(ref currentViewModel, value); }
        }

        public MainViewModel MainViewModel
        {
            get { return mainViewModel; }
            set { SetProperty(ref mainViewModel, value); }
        }
    }
}