using LiveCharts;
using LiveCharts.Wpf;
using SharedItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    public class PatientHistoryViewModel : CustomObservableObject
    {
        public PatientBike PatientBike { get; }
        public string Title { get; set; }
        public ObservableCollection<string> History { get; set; }
        public SeriesCollection SpeedCollection { get; set; }
        private ChartValues<double> speedChartValues;

        public PatientHistoryViewModel(PatientBike patientBike)
        {
            History = new ObservableCollection<string>();
            SpeedCollection = new SeriesCollection();
            speedChartValues = new ChartValues<double>();

            PatientBike = patientBike;
            Title = "History of " + patientBike.Username;

            InitializeLog();
            SpeedCollection.Add(new LineSeries(speedChartValues));
        }

        private void InitializeLog()
        {
            int i = 0;
            string result = "";
            foreach (DataSet dataSet in PatientBike.HistoricalData)
            {

                switch (dataSet.UpdateType)
                {
                    case UpdateType.Heartrate:
                        result += "HeartRate: " + dataSet.Value + " BPM";
                        break;
                    case UpdateType.Speed:
                        result += "Speed: " + dataSet.Value + " km/u";
                        // Fill the speed collection
                        speedChartValues.Add((double)dataSet.Value);
                        break;
                    case UpdateType.AccumulatedDistance:
                        result += "Distance: " + dataSet.Value + " km";
                        break;
                    case UpdateType.ElapsedTime:
                        var timeSpan = TimeSpan.FromSeconds((double)dataSet.Value);
                        result += "ElapsedTime: " + timeSpan.ToString(@"hh\:mm\:ss");
                        break;
                    case UpdateType.Resistance:
                        result += "Resistance: " + dataSet.Value + " %";
                        break;
                    case UpdateType.AccumulatedPower:
                        result += "AccumulatedPower: " + dataSet.Value;
                        break;
                    case UpdateType.InstantaniousPower:
                        result += "InstantaniousPower: " + dataSet.Value;
                        break;
                    default:
                        break;
                }

                result += ", ";
                i++;

                if (i % 5 == 0)
                {
                    result.Remove(result.Length - 2);
                    History.Add(dataSet.DateStamp.ToString() + " - " + result);
                    result = "";
                }
            }
        }
    }
}