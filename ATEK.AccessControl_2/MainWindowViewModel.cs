using ATEK.AccessControl_2.Login;
using ATEK.AccessControl_2.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase currentViewModel;
        private LoginViewModel loginViewModel = new LoginViewModel();
        private MainViewModel mainViewModel = new MainViewModel();

        public MainWindowViewModel()
        {
            NavLoginCommand = new RelayCommand<string>(OnNavLogin);
            NavMainCommand = new RelayCommand<string>(OnNavMain);
        }

        private void OnNavMain(string obj)
        {
            if (CurrentViewModel.GetType() != typeof(MainViewModel))
            {
                CurrentViewModel = mainViewModel;
                Console.WriteLine("Load OnNavMain");
            }
        }

        private void OnNavLogin(string obj)
        {
            if (CurrentViewModel.GetType() != typeof(LoginViewModel))
            {
                CurrentViewModel = loginViewModel;
                Console.WriteLine("Load OnNavLogin");
            }
        }

        public RelayCommand<string> NavLoginCommand { get; private set; }
        public RelayCommand<string> NavMainCommand { get; private set; }

        public ViewModelBase CurrentViewModel
        {
            get { return currentViewModel; }
            set { SetProperty(ref currentViewModel, value); }
        }

        public void LoadView()
        {
            CurrentViewModel = loginViewModel;
        }
    }
}