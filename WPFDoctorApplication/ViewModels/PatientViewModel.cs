using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;
using WPFDoctorApplication.Views;

namespace WPFDoctorApplication.ViewModels
{
    public class PatientViewModel : CustomObservableObject
    {
        private readonly DoctorClient doctorClient;

        public PatientBike PatientBike { get; set; }
        public double Speed { get; }

        //public double Speed { get {return PatientBike.Speed; } set { Speed = value; } }

        public string Username { get; set; }
        public ICommand StopCommand{ get; set; }
        public PatientViewModel(PatientBike patientBike, DoctorClient doctorClient)
        {
            this.PatientBike = patientBike;
            this.doctorClient = doctorClient;
            this.Speed = patientBike.Speed;
            this.Username = patientBike.Username;

            StopCommand = new RelayCommand(EmergencyStop);
        }

        public void SendResistance()
        {
            doctorClient.sendResistance(PatientBike.ResistanceValue + "", PatientBike.Username);
        }

        private void EmergencyStop()
        {
            PatientBike.ResistanceValue = 100;
            doctorClient.sendResistance(PatientBike.ResistanceValue + "", PatientBike.Username);
        }
    }
}
