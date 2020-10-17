using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WPFDoctorApplication.Utils
{
    public class CustomObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
