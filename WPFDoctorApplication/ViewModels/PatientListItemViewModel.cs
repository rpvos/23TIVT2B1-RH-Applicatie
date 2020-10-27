using System;
using System.Collections.Generic;
using System.Text;
using WPFDoctorApplication.Models;

namespace WPFDoctorApplication.ViewModels
{
    public class PatientListItemViewModel
    {
        public PatientBike PatientBike { set; get; }
        public PatientListItemViewModel(PatientBike patientBike)
        {
            this.PatientBike = patientBike;
        }
    }
}
