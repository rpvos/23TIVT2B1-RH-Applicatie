using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.Utils;

namespace WPFDoctorApplication.ViewModels
{
    public class PatientListViewModel : CustomObservableObject
    {
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
        //public ObservableCollection<PatientViewModel> ActivePatientViewModels { get; set; }
        private PatientBike _selectedPatientBike;

        public PatientBike SelectedPatientBike
        {
            get { return _selectedPatientBike; }
            set { _selectedPatientBike = value;
                SelectedPatientViewModel = new PatientViewModel(value);
            }
        }


        public PatientListViewModel(ShellViewModel shellViewModel)
        {
            this.shellViewModel = shellViewModel;
            this.PatientBikeList = shellViewModel.PatientBikeList;

            //this.PatientViewModelList = new ObservableCollection<PatientViewModel>(this.patientBikeList.Select(md => new PatientViewModel(md)));
            //this.PatientViewModelList = this.patientBikeList.ConvertAll(x => new PatientViewModel(x));
            //this.PatientViewModelList.Add(new PatientViewModel(new PatientBike("test")));
        }
    }
}
