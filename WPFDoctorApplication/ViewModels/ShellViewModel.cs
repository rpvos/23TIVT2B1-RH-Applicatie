using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using WPFDoctorApplication.Utils;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.ViewModels;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace WPFDoctorApplication.ViewModels
{
    public class ShellViewModel : CustomObservableObject
    {
        public ICommand PatientViewCommand { get; set; }
        public CustomObservableObject SelectedViewModel { get; set; }
        public ObservableCollection<PatientBike> PatientBikeList { get; set; }
        public DoctorClient DoctorClient { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
        public CustomObservableObject PatientListViewModel { get; set; }
        public string DebugMessage { get; set; }


        public ShellViewModel()
        {
            Initialize();

            //TEST
            PatientBikeList.Add(new PatientBike(DoctorClient, "TEST"));
        }
        public void Initialize()
        {
            this.PatientBikeList = new ObservableCollection<PatientBike>();
            this.DoctorClient = new DoctorClient(this);
            this.LoginViewModel = new LoginViewModel(this, DoctorClient);

            this.SelectedViewModel = LoginViewModel;
            //var uiContext = SynchronizationContext.Current;

        }

        public void OnLoginSucces()
        {
            this.PatientListViewModel = new PatientListViewModel(this);
            this.SelectedViewModel = this.PatientListViewModel;
        }
    }
}
