using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.Models
{
    /// <summary>
    /// Acts as model and viewModel for PatientViewModel, rather have
    /// it separated, but there are problems with the databinding which I don't understand why.
    /// This is the reason why PatientView databinding starts with "PatientBike.".
    /// </summary>
    public class PatientBike : CustomObservableObject
    {
        private string _privateChatMessage;
        private DoctorClient doctorClient;

        public string Username { get; set; }
        public double Speed { get; set; }
        public double DistanceTraveled { get; set; }
        public double AccumulatedPower { get; set; }
        public double ElapsedTime { get; set; }
        public int Resistance { get; set; }
        public double HeartRate { get; set; }
        public ObservableCollection<string> PrivateChatList { get; set; }
        public ICommand PrivateChatKeyDownCommand { get; set; }

        public string PrivateChatMessage { get { return _privateChatMessage; } set { _privateChatMessage = value;  } }

        public PatientBike(DoctorClient doctorClient, string username)
        {
            this.Username = username;
            PrivateChatList = new ObservableCollection<string>();
            PrivateChatKeyDownCommand = new RelayCommand(() => PrivateChatKeyDown()); ;
            this.doctorClient = doctorClient;
        }

        private void PrivateChatKeyDown()
        {
            PrivateChatList.Add("Doctor: " + PrivateChatMessage);
            doctorClient.sendPrivMessage(PrivateChatMessage, Username);
            PrivateChatMessage = "";
        }
    }
}
