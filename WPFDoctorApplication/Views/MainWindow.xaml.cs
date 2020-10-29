using System.Windows;
using WPFDoctorApplication.Utils;
using WPFDoctorApplication.ViewModels;

namespace WPFDoctorApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICloseable
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ShellViewModel();
        }
    }
}
