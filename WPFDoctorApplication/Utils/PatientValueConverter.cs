using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace WPFDoctorApplication.Utils
{
    class PatientValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //"targetType" may have had use here
            string type = (string)parameter;
            string convertedValue = "";
            switch (type)
            {
                case "SPEED":
                    convertedValue = value + " km/u";
                    break;
                case "DISTANCETRAVELED":
                    convertedValue = value + " km";
                    break;
                case "ELAPSEDTIME":
                    var timeSpan = TimeSpan.FromSeconds((double)value);                   
                    convertedValue = timeSpan.ToString(@"hh\:mm\:ss");
                    break;
                case "HEARTRATE":
                    convertedValue = value + " BPM";
                    break;
                default:
                    break;
            }

            return convertedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
