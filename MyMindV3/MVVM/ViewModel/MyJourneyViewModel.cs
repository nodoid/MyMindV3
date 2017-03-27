using GalaSoft.MvvmLight.Views;
using MvvmFramework.Models;
using MvvmFramework.Webservices;
using System.Collections.Generic;
using System.Linq;
using System;
using MvvmFramework.Enums;
using MvvmFramework.Interfaces;

namespace MvvmFramework.ViewModel
{
    public class MyJourneyViewModel : BaseViewModel
    {
        INavigationService navService;
        IConnectivity connectService;
        IDialogService diaService;

        public MyJourneyViewModel(INavigationService nav, IConnectivity con, IDialogService dia)
        {
            navService = nav;
            connectService = con;
            diaService = dia;

            if (con.IsConnected)
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Journey_Page_View : ActionCodes.User_My_Journey_Page_View);
        }

        public string ClientID { get; set; }
        public string HCP { get; set; }

        IEnumerable<Appointment> appointments;
        public IEnumerable<Appointment> Appointments
        {
            get { return appointments; }
            set {
                Set(() => Appointments, ref appointments, value);
                GenerateAppointments();
            }
        }

        public string ApptsThisWeek { get; set; }
        public string ApptsThisMonth { get; set; }

        public void GetAppointments()
        {
            if (connectService.IsConnected)
            {
                Appointments = new UsersWebservice().GetPatientAppointments(ClientID, HCP).Result;
                if (Appointments != null)
                {
                    var sortedList = Appointments.OrderByDescending(o => o.AppointmentDateTime.CleanDate()).ToList();

                    var dayOfYear = DateTime.Now.DayOfYear;
                    var lower = dayOfYear - (int)DayOfWeek.Monday;
                    var upper = dayOfYear + (int)DayOfWeek.Friday;
                    var thisWeek = 0;

                    var thisMonth = sortedList?.Count(t => t.AppointmentDateTime.CleanDate().Month == DateTime.Now.Month);

                    if (thisMonth != 0)
                    {
                        // we have an appointment this month
                        var thisMonthsApps = sortedList.Where(t => t.AppointmentDateTime.CleanDate().Month == DateTime.Now.Month).ToList();
                        foreach (var app in thisMonthsApps)
                        {
                            var date = app.AppointmentDateTime.CleanDate();
                            var dayOfMonth = date.DayOfYear;
                            if (dayOfMonth >= lower && dayOfMonth <= upper)
                                thisWeek++;
                        }
                    }

                    ApptsThisWeek = thisWeek.ToString();
                    ApptsThisMonth = thisMonth.ToString();
                }
            }
            else
            {
                diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
                ApptsThisMonth = ApptsThisWeek = "0";
            }
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
