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

namespace WPFDoctorApplication.ViewModels
{
    public class ShellViewModel : MyObservableObject
    {
        public ICommand PatientViewCommand { get; set; }
        public MyObservableObject SelectedViewModel { get; set; }
        public List<PatientBike> PatientBikeList { get; set; }
        public DoctorClient DoctorClient { get; set; }


        public ShellViewModel()
        {
            Initialize();   
        }
        public void Initialize()
        {
            this.PatientBikeList = new List<PatientBike>();
            this.DoctorClient = new DoctorClient(PatientBikeList);
            this.SelectedViewModel = new LoginViewModel();
            PatientViewCommand = new RelayCommand(() =>
            {
                SelectedViewModel = new PatientViewModel(new PatientBike("test"));
            });
        }
    }
}
