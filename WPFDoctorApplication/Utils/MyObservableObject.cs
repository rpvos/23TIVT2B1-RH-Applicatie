using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WPFDoctorApplication.Utils
{
    public class MyObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
