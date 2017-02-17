using GalaSoft.MvvmLight.Views;
using MvvmFramework.Models;
using MvvmFramework.Webservices;
using System.Collections.Generic;
using System.Linq;
using System;
using MvvmFramework.Enums;

namespace MvvmFramework.ViewModel
{
    public class MyJourneyViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyJourneyViewModel(INavigationService nav)
        {
            navService = nav;
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Journey_Page_View : ActionCodes.User_My_Journey_Page_View);
        }

        public string ClientID { get; set; }
        public string HCP { get; set; }

        IEnumerable<Appointment> appointments;
        public IEnumerable<Appointment> Appointments
        {
            get { return appointments; }
            set
            {
                Set(() => Appointments, ref appointments, value);
                GenerateAppointments();
            }
        }

        public void GetAppointments()
        {
            Appointments = new UsersWebservice().GetPatientAppointments(ClientID, HCP).Result;
        }

        IEnumerable<Appointment> pastAppointments;
        public IEnumerable<Appointment> PastAppointments
        {
            get { return pastAppointments; }
            set { Set(() => PastAppointments, ref pastAppointments, value); }
        }

        IEnumerable<Appointment> nextAppointments;
        public IEnumerable<Appointment> NextAppointments
        {
            get { return nextAppointments; }
            set { Set(() => NextAppointments, ref nextAppointments, value); }
        }

        void GenerateAppointments()
        {
            if (Appointments != null)
            {
                var appts = Appointments.ToList();
                appts = appts.OrderBy(t => t.ApptDateTime.Year).ThenBy(t => t.ApptDateTime.Month).ThenBy(t => t.ApptDateTime.Day).ToList();

                var today = DateTime.Now;
                var monday = DateTime.Now;
                if (today.DayOfWeek > DayOfWeek.Monday)
                {
                    var sub = (int)today.DayOfWeek;
                    monday = monday.AddDays(-sub);
                }

                var past = appts.Where(t => t.ApptDateTime < monday) as IEnumerable<Appointment>;
                var future = appts.Where(t => t.ApptDateTime >= monday) as IEnumerable<Appointment>;

                PastAppointments = past;
                NextAppointments = future;
            }
        }
    }
}
