using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.Models
{
    /// <summary>
    /// Represents the model for PatientViewModel and PatientView. 
    /// Contains all the information and methods for the patient, whom the doctor is monitoring.
    /// </summary>
    public class PatientBike : CustomObservableObject
    {
        private double _speed;

        //This has to change for performance
        public DoctorClient DoctorClient;

        public double Speed
        {
            get { return _speed; }
            set
            { 
                _speed = value;
                //SpeedValues.RemoveAt(0);
                //SpeedValues.Add(value);
            }
        }

        public string Username { get; set; }
        //public double Speed { get; set; }
        public double DistanceTraveled { get; set; }
        public double AccumulatedPower { get; set; }
        public double ElapsedTime { get; set; }
        public int ResistanceValue { get; set; }
        public double HeartRate { get; set; }
        public ObservableCollection<string> PrivateChatList { get; set; }
        public string PrivateChatMessage { get; set; }
        public Func<double, string> SpeedYFormatter { get; set; }
        public ChartValues<double> SpeedValues { get; set; }
        public PatientBike(DoctorClient doctorClient, string username)
        {
            this.Username = username;
            PrivateChatList = new ObservableCollection<string>();
            this.DoctorClient = doctorClient;
            SpeedValues = new ChartValues<double> { 0, 0, 0, 0, 0 };
        }
        public void SendMessage()
        {
            PrivateChatList.Add("Doctor: " + PrivateChatMessage);
            DoctorClient.sendPrivMessage(PrivateChatMessage, Username);
            PrivateChatMessage = "";
        }       
    }
}
