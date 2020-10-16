using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace WPFDoctorApplication.ViewModels
{
    public class ShellViewModel
    {
        private PatientViewModel _patientViewModel;

        public ICommand PatientViewCommand
        {
            get;
            set;
        }
        public PatientViewModel PatientViewModel
        {
            get
            {
                return _patientViewModel;
            }
    set
            {
                _patientViewModel = value;
                //NotifyOfPropertyChange(() => PatientViewModel);
            }
        }

        public ShellViewModel()
        {
            //PatientViewModel = new PatientViewModel();
            PatientViewModel = null;
            PatientViewCommand = new RelayCommand(() =>
            {                
                PatientViewModel = new PatientViewModel();
            });
        }

        //public void Test()
        //{
            
        //}
    }
}
