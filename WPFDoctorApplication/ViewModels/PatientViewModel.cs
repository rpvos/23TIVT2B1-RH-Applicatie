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
        public double Speed { get; }

        //public double Speed { get {return PatientBike.Speed; } set { Speed = value; } }

        public string Username { get; set; }
        public PatientViewModel(PatientBike patientBike)
        {
            this.PatientBike = patientBike;
            this.Speed = patientBike.Speed;
            this.Username = patientBike.Username;
        }
    }
}
