using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;
using WPFDoctorApplication.Views;

namespace WPFDoctorApplication.ViewModels
{
    public class PatientViewModel : CustomObservableObject
    {
        private readonly DoctorClient doctorClient;
        private bool IsReading = true;
        public PatientBike PatientBike { get; set; }
        public string Username { get; set; }
        public ICommand StopCommand{ get; set; }
        public ICommand PrivateChatKeyDownCommand { get; set; }
        public ICommand StartSessionCommand { get; set; }
        public ICommand GetHistoricalDataCommand { get; set; }
        public string[] SpeedLabels { get; set; }
        public SeriesCollection SpeedCollection { get; set; }

        public PatientViewModel(PatientBike patientBike, DoctorClient doctorClient)
        {
            this.PatientBike = patientBike;
            this.doctorClient = doctorClient;
            Username = patientBike.Username;
            PrivateChatKeyDownCommand = new RelayCommand(PatientBike.SendMessage);
            StopCommand = new RelayCommand(PatientBike.EmergencyStop);
            StartSessionCommand = new RelayCommand(PatientBike.StartSession);
            GetHistoricalDataCommand = new RelayCommand(PatientBike.AskUserDataFromServer);

            InitializeGraphs();
        }

        private void Read()
        {
            while (IsReading)
            {
                Thread.Sleep(250);
                App.Current.Dispatcher.Invoke(() =>
                {
                    PatientBike.SpeedValues.Add(PatientBike.Speed);

                    // Use the last 15 values
                    if (PatientBike.SpeedValues.Count > 15)
                        PatientBike.SpeedValues.RemoveAt(0);
                });
            }
        }

        private void InitializeGraphs()
        {
            SpeedCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Speed",
                    Values = PatientBike.SpeedValues
                }
            };

            SpeedLabels = new[] { "?", "?", "?", "?", "?" };
            //SpeedYFormatter = value => value.ToString();

            Task.Run(Read);
        }
    }
}
