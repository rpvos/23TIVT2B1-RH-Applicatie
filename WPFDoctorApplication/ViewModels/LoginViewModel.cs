using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Input;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    /// <summary>
    /// ViewModel for login
    /// </summary>
    public class LoginViewModel : CustomObservableObject
    {
        private readonly DoctorClient doctorClient;
        public Visibility LoginVisibility { get; set; }
        public string Username { get; set; } = "dokter";
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
        public ICommand LoginCommand { get; set; }
        public ShellViewModel ShellViewModel { get; }

        public LoginViewModel(ShellViewModel shellViewModel, DoctorClient doctorClient)
        {
            doctorClient.StartClient();
            ShellViewModel = shellViewModel;
            this.doctorClient = doctorClient;
            LoginCommand = new RelayCommand(() =>
            {
                Login();
            });
        }

        public void LoginFailed()
        {
            ErrorMessage = "Please try again.";
        }

        public void LoginSucces()
        {
            LoginVisibility = Visibility.Collapsed;
            ShellViewModel.OnLoginSucces();
        }

        private void Login()
        {
            doctorClient.senderUserCredentials(Username, Password);
        }
    }
}
