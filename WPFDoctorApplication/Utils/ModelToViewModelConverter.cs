using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.ViewModels;

namespace WPFDoctorApplication.Utils
{
    public class ModelToViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var convertable = value as List<PatientBike>;
            if (value != null)
            {
                List<PatientBike> patientList = (List<PatientBike>)value;
                List<PatientViewModel> convertedList = patientList.ConvertAll(x => new PatientViewModel(x));
                return convertedList;
            }
            else if (value as PatientBike != null)
            {
                return new PatientViewModel((PatientBike)value);
            }
            //else if (value as ObservableCollection<PatientBike> != null)
            //{
            //    ObservableCollection<PatientBike> convertableList = (ObservableCollection<PatientBike>)value;
            //    convertableList.ConvertAll
            //    return new PatientViewModel((PatientBike)value);
            //}
            throw new InvalidCastException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
