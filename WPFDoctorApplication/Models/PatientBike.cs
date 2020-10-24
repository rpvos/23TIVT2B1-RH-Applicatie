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
    /// Acts as model and viewModel for PatientViewModel, rather have
    /// it separated, but there are problems with the databinding which I don't understand why.
    /// This is the reason why PatientView databinding starts with "PatientBike.".
    /// </summary>
    public class PatientBike : CustomObservableObject
    {
        private double _speed;

        //This has to change for performance
        private bool IsReading = true;
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
        public ICommand PrivateChatKeyDownCommand { get; set; }

        public string PrivateChatMessage { get; set; }
        public SeriesCollection SpeedCollection { get; set; }
        public string[] SpeedLabels { get; set; }
        public Func<double, string> SpeedYFormatter { get; set; }
        public ChartValues<double> SpeedValues { get; set; }
        public PatientBike(DoctorClient doctorClient, string username)
        {
            this.Username = username;
            PrivateChatList = new ObservableCollection<string>();
            PrivateChatKeyDownCommand = new RelayCommand(() => PrivateChatKeyDown()); ;
            this.DoctorClient = doctorClient;

            InitializeGraphs();
        }
    

        private void PrivateChatKeyDown()
        {
            PrivateChatList.Add("Doctor: " + PrivateChatMessage);
            DoctorClient.sendPrivMessage(PrivateChatMessage, Username);
            PrivateChatMessage = "";
        }

        private void InitializeGraphs()
        {
            SpeedValues = new ChartValues<double> { 0, 0, 0, 0, 0 };
            SpeedCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Speed",
                    Values = SpeedValues
                }
            };

            SpeedLabels = new[] { "?", "?", "?", "?", "?" };
            //SpeedYFormatter = value => value.ToString();

            Task.Factory.StartNew(Read);
        }

        private void Read()
        {

            while (IsReading)
            {
                Thread.Sleep(250);

                SpeedValues.Add(Speed);

                //lets only use the last 30 values
                if (SpeedValues.Count > 30) 
                    SpeedValues.RemoveAt(0);
            }
        }
    }
}
