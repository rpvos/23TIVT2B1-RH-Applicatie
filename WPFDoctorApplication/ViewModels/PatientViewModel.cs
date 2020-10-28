using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        public ObservableCollection<string> TimeLabels { get; set; }
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

                    // Use the last 40 values
                    if (PatientBike.SpeedValues.Count > 40)
                        PatientBike.SpeedValues.RemoveAt(0);

                    TimeLabels.Add(TimeSpan.FromSeconds((double)PatientBike.ElapsedTime).ToString(@"hh\:mm\:ss"));
                    if (TimeLabels.Count > 20)
                    {
                        TimeLabels.RemoveAt(0);
                    }
                });
            }
        }

        private void InitializeGraphs()
        {
            for (int i = 0; i < 40; i++)
            {
                PatientBike.SpeedValues.Add(0);
            }

            TimeLabels = new ObservableCollection<string>();
            for (int i = 0; i < 40; i++)
            {
                TimeLabels.Add("00:00:00");
            }

            SpeedCollection = new SeriesCollection
            {
                new LineSeries
                {
                    PointGeometry = null,
                    Title = "Speed",
                    Values = PatientBike.SpeedValues
                }
            };


            //SpeedYFormatter = value => value.ToString();

            Task.Run(Read);
        }
    }
}
