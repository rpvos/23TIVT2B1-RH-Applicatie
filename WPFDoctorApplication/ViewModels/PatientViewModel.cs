using System;
using System.Collections.Generic;
using System.Text;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;
using WPFDoctorApplication.Views;

namespace WPFDoctorApplication.ViewModels
{
    public class PatientViewModel : CustomObservableObject
    {
        public PatientBike PatientBike { get; set; }
        public double Speed { get; set; }
        public string Username { get; set; }
        public PatientViewModel(PatientBike patientBike)
        {
            this.PatientBike = patientBike;
            this.Speed = this.PatientBike.Speed;
            this.Username = this.PatientBike.Username + " model";
        }
    }
}
