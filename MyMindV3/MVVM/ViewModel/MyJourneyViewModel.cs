using GalaSoft.MvvmLight.Views;
using MvvmFramework.Models;
using MvvmFramework.Webservices;
using System.Collections.Generic;

namespace MvvmFramework.ViewModel
{
    public class MyJourneyViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyJourneyViewModel(INavigationService nav)
        {
            navService = nav;
        }

        public string ClientID { get; set; }
        public string HCP { get; set; }

        IEnumerable<Appointment> appointments;
        public IEnumerable<Appointment> Appointments
        {
            get { Appointments = new UsersWebservice().GetPatientAppointments(ClientID, HCP).Result;  return appointments; }
            set { Set(() => Appointments, ref appointments, value); }
        }
    }
}
