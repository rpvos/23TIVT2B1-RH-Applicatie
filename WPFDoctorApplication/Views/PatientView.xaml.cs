using System.Windows;
using System.Windows.Controls;
using WPFDoctorApplication.ViewModels;

namespace WPFDoctorApplication.Views
{
    /// <summary>
    /// Interaction logic for PatientView.xaml
    /// </summary>
    public partial class PatientView : UserControl
    {
        public PatientView()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PatientViewModel patientViewModel = (PatientViewModel)DataContext;
            patientViewModel.PatientBike.SendResistance();
        }
    }
}
