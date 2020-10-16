using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using WPFDoctorApplication.Utils;
using WPFDoctorApplication.Models;

namespace WPFDoctorApplication.ViewModels
{
    public class ShellViewModel : MyObservableObject
    {
        public ICommand PatientViewCommand { get; set; }
        public MyObservableObject SelectedPatientViewModel { get; set; }
        public List<PatientBike> PatientBikeList;


        public ShellViewModel(List<PatientBike> patientBikeList)
        {
            this.PatientBikeList = patientBikeList;
            PatientViewCommand = new RelayCommand(() =>
            {                
                SelectedPatientViewModel = new PatientViewModel(new PatientBike("test"));
            });
        }
    }
}
