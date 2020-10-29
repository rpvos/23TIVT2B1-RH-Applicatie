using LiveCharts;
using LiveCharts.Wpf;
using SharedItems;
using System;
using System.Collections.ObjectModel;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    /// <summary>
    /// View model for displaying the patient history
    /// </summary>
    public class PatientHistoryViewModel : CustomObservableObject
    {
        public PatientBike PatientBike { get; }
        public string Title { get; set; }
        public ObservableCollection<string> History { get; set; }
        public SeriesCollection SpeedCollection { get; set; }
        private ChartValues<double> speedChartValues;
        public SeriesCollection HeartRateCollection { get; set; }
        private ChartValues<int> heartRateChartValues;

        public PatientHistoryViewModel(PatientBike patientBike)
        {
            History = new ObservableCollection<string>();
            speedChartValues = new ChartValues<double>();
            heartRateChartValues = new ChartValues<int>();

            PatientBike = patientBike;
            Title = "History of " + patientBike.Username;

            InitializeLog();

            SpeedCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Speed",
                    Values = speedChartValues
                }
            };

            HeartRateCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Heart rate",
                    Values = heartRateChartValues
                }
            };
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
                        // Fill the speed collection
                        heartRateChartValues.Add((int)dataSet.Value);
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