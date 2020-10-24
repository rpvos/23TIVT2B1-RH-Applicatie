using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    public class PatientListViewModel : CustomObservableObject
    {
        private PatientBike _selectedPatientBike;
        private ShellViewModel shellViewModel;
        private ObservableCollection<PatientBike> _patientBikeList;
        public ObservableCollection<PatientBike> PatientBikeList
        { 
            get
            {
                return _patientBikeList;
            }
             set 
            {
                _patientBikeList = value;
                //OnPropertyChanged("PatientBikeList");
            } 
        }
        public CustomObservableObject SelectedPatientViewModel { get; set; }
        public string GlobalChatMessage { get; set; }
        public ICommand GlobalChatKeyDownCommand { get; set; }
        public ICommand QuitCommand { get; set; }
        public ObservableCollection<string> GlobalChatList { get; set; }
        public PatientBike SelectedPatientBike
        {
            get { return _selectedPatientBike; }
            set { _selectedPatientBike = value;
                SelectedPatientViewModel = new PatientViewModel(value, value.DoctorClient);
            }
        }

        public PatientListViewModel(ShellViewModel shellViewModel)
        {
            this.shellViewModel = shellViewModel;
            this.PatientBikeList = shellViewModel.PatientBikeList;
            GlobalChatList = new ObservableCollection<string>();
            GlobalChatKeyDownCommand = new RelayCommand(() => GlobalChatKeyDown()); ;
            QuitCommand = new RelayCommand(Quit);
        }

        private void GlobalChatKeyDown()
        {
            GlobalChatList.Add("Global?? Doctor: " + GlobalChatMessage);
            shellViewModel.DoctorClient.sendGlobalChatMessage(GlobalChatMessage);
            GlobalChatMessage = "";
        }

        private void Quit()
        {

        }
    }
}
