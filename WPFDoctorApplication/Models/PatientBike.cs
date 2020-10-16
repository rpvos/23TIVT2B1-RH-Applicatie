using System;
using System.Collections.Generic;
using System.Text;

namespace WPFDoctorApplication.Models
{
    public class PatientBike
    {
        public string Username { get; set; }
        public double Speed { get; set; }

        public PatientBike(string username)
        {
            this.Username = username;
        }
    }
}
