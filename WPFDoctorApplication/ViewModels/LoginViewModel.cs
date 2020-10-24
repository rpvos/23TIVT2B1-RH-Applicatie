using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    public class LoginViewModel : CustomObservableObject
    {
        private readonly DoctorClient doctorClient;
        public Visibility LoginVisibility { get; set; }
        public string Username { get; set; } = "dokter";
        public string Password { get; set; } = "123";
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
            Password = "";
            ErrorMessage = "Please try again.";
        }

        public void LoginSucces()
        {
            LoginVisibility = Visibility.Collapsed;
            ShellViewModel.OnLoginSucces();
        }

        private void Login()
        {
            //Questionable threading
            //Thread doctorClientThread = new Thread(() => doctorClient.StartClient(Username, Password));
            //doctorClientThread.Start();            
            doctorClient.senderUserCredentials(Username, Password);
        }
    }
}
