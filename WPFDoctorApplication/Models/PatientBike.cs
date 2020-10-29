using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using SharedItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFDoctorApplication.Utils;
using WPFDoctorApplication.ViewModels;
using WPFDoctorApplication.Views;

namespace WPFDoctorApplication.Models
{
    /// <summary>
    /// Represents the model for PatientViewModel and PatientView. 
    /// Contains all the information and methods for the patient, whom the doctor is monitoring.
    /// </summary>
    public class PatientBike : CustomObservableObject
    {
        private bool isReading = true;
        private double _speed;
        private string _sessionText = "Start session";
        private bool _isInSession = false;
        public DoctorClient DoctorClient;
        public List<DataSet> HistoricalData{ get; set; }
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
        public bool IsInSession {
            get 
            {
                return _isInSession;
            } 
            set 
            {
                _isInSession = value;
                if (value)
                    SessionText = "Stop session";
                else
                    SessionText = "Start session";                
            } 
        }
        public string SessionText { 
            get
            {
                return _sessionText;
            } 
            set 
            {
                _sessionText = value;
            } 
        } 
        public string Username { get; set; }
        //public double Speed { get; set; }
        public double DistanceTraveled { get; set; }
        public double AccumulatedPower { get; set; }
        public double ElapsedTime { get; set; }
        public int ResistanceValue { get; set; }
        public double HeartRate { get; set; } = 50;
        public ObservableCollection<string> PrivateChatList { get; set; }
        public string PrivateChatMessage { get; set; }
        public ChartValues<double> SpeedValues { get; set; }
        public ChartValues<double> DistanceValues { get; set; }
        public ObservableCollection<string> TimeLabels { get; set; }
        public PatientBike(DoctorClient doctorClient, string username)
        {
            this.Username = username;
            this.DoctorClient = doctorClient;
            PrivateChatList = new ObservableCollection<string>();
            SpeedValues = new ChartValues<double>();
            DistanceValues = new ChartValues<double>();
            HistoricalData = new List<DataSet>();

            InitializeGraphs();
        }
        public void SendMessage()
        {
            PrivateChatList.Add("Doctor: " + PrivateChatMessage);
            DoctorClient.sendPrivMessage(PrivateChatMessage, Username);
            PrivateChatMessage = "";
        }       

        public void StartSession()
        {
            IsInSession = !IsInSession;
            this.DoctorClient.sendInSession(IsInSession, Username);
        }

        public void AskUserDataFromServer()
        {
            HistoricalData.Clear();
            this.DoctorClient.AskUserData(Username);
        }
        public void SendResistance()
        {
            DoctorClient.sendResistance(ResistanceValue + "", Username);
        }

        public void EmergencyStop()
        {
            ResistanceValue = 100;
            DoctorClient.sendResistance(ResistanceValue + "", Username);
        }

        public void AddDataSet(DataSet data)
        {
            HistoricalData.Add(data);
        }

        public void ShowHistoricalDataWindow()
        {
            PatientHistoryWindow patientHistoryWindow = new PatientHistoryWindow();
            patientHistoryWindow.DataContext = new PatientHistoryViewModel(this);
            patientHistoryWindow.Show();
        }

        private void InitializeGraphs()
        {
            for (int i = 0; i < 40; i++)
            {
                SpeedValues.Add(0);
                DistanceValues.Add(0);
            }

            TimeLabels = new ObservableCollection<string>();
            for (int i = 0; i < 40; i++)
            {
                TimeLabels.Add("00:00:00");
            }

        Task.Run(Read);
        }

        private void Read()
        {
            while (isReading)
            {
                Thread.Sleep(250);
                App.Current.Dispatcher.Invoke(() =>
                {
                    SpeedValues.Add(Speed);

                    // Use the last 40 values
                    if (SpeedValues.Count > 40)
                        SpeedValues.RemoveAt(0);

                    DistanceValues.Add(DistanceTraveled);

                    if (DistanceValues.Count > 40)
                        DistanceValues.RemoveAt(0);

                    TimeLabels.Add(TimeSpan.FromSeconds((double)ElapsedTime).ToString(@"hh\:mm\:ss"));
                    if (TimeLabels.Count > 20)
                    {
                        TimeLabels.RemoveAt(0);
                    }
                });
            }
        }
    }
}
