using System;
using System.Collections.Generic;
using System.Text;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.Models
{
    public class PatientBike : CustomObservableObject
    {
        public string Username { get; set; }
        public double Speed { get; set; }
        public double DistanceTraveled { get; set; }
        public double AccumulatedPower { get; set; }
        public double ElapsedTime { get; set; }
        public int Resistance { get; set; }

        public PatientBike(string username)
        {
            this.Username = username;
        }
    }
}
