using System;
using System.Collections.Generic;
using System.Text;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;
using WPFDoctorApplication.Views;

namespace WPFDoctorApplication.ViewModels
{
    public class PatientViewModel : MyObservableObject
    {
        public PatientBike PatientBike { get; set; }
        public PatientViewModel(PatientBike patientBike)
        {
            this.PatientBike = patientBike;
        }
    }
}
