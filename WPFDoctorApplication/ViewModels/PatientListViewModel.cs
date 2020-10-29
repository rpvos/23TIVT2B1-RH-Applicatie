using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    /// <summary>
    /// This class is responsable for showing the patient bike in the list
    /// </summary>
    public class PatientListViewModel : CustomObservableObject
    {
        private PatientBike _selectedPatientBike;
        private ShellViewModel shellViewModel;

        public ObservableCollection<PatientBike> PatientBikeList { get; set; }
        public CustomObservableObject SelectedPatientViewModel { get; set; }
        public string GlobalChatMessage { get; set; }
        public ICommand GlobalChatKeyDownCommand { get; set; }
        public ObservableCollection<string> GlobalChatList { get; set; }
        public PatientBike SelectedPatientBike
        {
            get { return _selectedPatientBike; }
            set
            {
                _selectedPatientBike = value;
                if (_selectedPatientBike != null)
                {
                    SelectedPatientViewModel = new PatientViewModel(value, value.DoctorClient);
                }
                else
                {
                    SelectedPatientViewModel = null;
                }
            }
        }

        public PatientListViewModel(ShellViewModel shellViewModel)
        {
            this.shellViewModel = shellViewModel;
            PatientBikeList = shellViewModel.PatientBikeList;
            GlobalChatList = new ObservableCollection<string>();
            GlobalChatKeyDownCommand = new RelayCommand(() => GlobalChatKeyDown()); ;
        }

        private void GlobalChatKeyDown()
        {
            GlobalChatList.Add("Doctor: " + GlobalChatMessage);
            shellViewModel.DoctorClient.sendGlobalChatMessage(GlobalChatMessage);
            GlobalChatMessage = "";
        }


    }
}
