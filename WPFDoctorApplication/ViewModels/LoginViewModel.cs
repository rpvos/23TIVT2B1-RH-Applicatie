using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    public class LoginViewModel : MyObservableObject
    {
        private DoctorClient doctorClient;
        public string Username { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
        public ICommand LoginCommand { get; set; }

        public LoginViewModel(DoctorClient doctorClient)
        {
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
            
        }

        private void Login()
        {
            doctorClient.StartClient(Username, Password);
        }
    }
}
