using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WPFDoctorApplication.ViewModels
{
    public class ShellViewModel
    {
        private PatientViewModel _patientViewModel;
        public PatientViewModel PatientViewModel
        {
            get
            {
                return _patientViewModel;
            }
            set
            {
                PatientViewModel = value;
                //NotifyOfPropertyChange(() => PatientViewModel);
            }
        }

        public ShellViewModel()
        {
            //PatientViewModel = new PatientViewModel();
            PatientViewModel = null;
        }
    }
}
