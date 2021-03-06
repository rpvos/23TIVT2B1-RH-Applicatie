﻿using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows.Input;
using System.Windows.Media;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    /// <summary>
    /// View model for displaying the patient information
    /// </summary>
    public class PatientViewModel : CustomObservableObject
    {
        private readonly DoctorClient doctorClient;
        public PatientBike PatientBike { get; set; }
        public string Username { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand PrivateChatKeyDownCommand { get; set; }
        public ICommand StartSessionCommand { get; set; }
        public ICommand GetHistoricalDataCommand { get; set; }
        public SeriesCollection SpeedCollection { get; set; }
        public SeriesCollection DistanceCollection { get; set; }
        public Func<double, string> DistanceFormatter { get; set; }


        public PatientViewModel(PatientBike patientBike, DoctorClient doctorClient)
        {
            PatientBike = patientBike;
            this.doctorClient = doctorClient;
            Username = patientBike.Username;

            Initialize();
        }

        private void Initialize()
        {
            PrivateChatKeyDownCommand = new RelayCommand(PatientBike.SendMessage);
            StopCommand = new RelayCommand(PatientBike.EmergencyStop);
            StartSessionCommand = new RelayCommand(PatientBike.StartSession);
            GetHistoricalDataCommand = new RelayCommand(PatientBike.AskUserDataFromServer);
            DistanceFormatter = value => value.ToString("0.000");
            SpeedCollection = new SeriesCollection
            {
                new LineSeries
                {
                    PointGeometry = null,
                    Title = "Speed in km/u",
                    Values = PatientBike.SpeedValues
                }
            };
            DistanceCollection = new SeriesCollection
            {
                new LineSeries
                {
                    PointGeometry = null,
                    Title = "Distance in km",
                    Values = PatientBike.DistanceValues,
                    Stroke = Brushes.Green
                }
            };
        }
    }
}
