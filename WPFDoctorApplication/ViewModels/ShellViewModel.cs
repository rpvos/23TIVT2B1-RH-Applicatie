using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    /// <summary>
    /// This is the main window
    /// </summary>
    public class ShellViewModel : CustomObservableObject
    {
        public ICommand PatientViewCommand { get; set; }
        public CustomObservableObject SelectedViewModel { get; set; }
        public ObservableCollection<PatientBike> PatientBikeList { get; set; }
        public DoctorClient DoctorClient { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
        public CustomObservableObject PatientListViewModel { get; set; }
        public string DebugMessage { get; set; }
        public ICommand QuitCommand { get; set; }
        public Visibility QuitVisibility { get; set; } = Visibility.Hidden;

        public ShellViewModel()
        {
            Initialize();
        }
        public void Initialize()
        {
            PatientBikeList = new ObservableCollection<PatientBike>();
            DoctorClient = new DoctorClient(this);
            LoginViewModel = new LoginViewModel(this, DoctorClient);
            QuitCommand = new RelayCommand<ICloseable>(Quit);

            SelectedViewModel = LoginViewModel;
        }

        public void OnLoginSucces()
        {
            PatientListViewModel = new PatientListViewModel(this);
            SelectedViewModel = PatientListViewModel;
            QuitVisibility = Visibility.Visible;
        }
        private void Quit(ICloseable window)
        {
            DoctorClient.disconnect();
            window.Close();
        }
    }
}
