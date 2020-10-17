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
        private double _speed;
        public PatientBike PatientBike { get; set; }
        public double Speed { get { return _speed; } set
            {
                _speed = value;
                OnPropertyChanged("Speed");
            } 
        }
        public string Username { get; set; }
        public PatientViewModel(PatientBike patientBike)
        {
            this.PatientBike = patientBike;
            this.Speed = patientBike.Speed;
            this.Username = patientBike.Username;
        }
    }
}
