using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MyMindV3.Languages;
using MvvmFramework.ViewModel;
using MvvmFramework.Models;

namespace MyMindV3.Views
{
    public partial class MyJourneyView : ContentPage
    {
        MyJourneyViewModel ViewModel => App.Locator.MyJourney;
        IEnumerable<Appointment> appointments;
        readonly string id;

        public MyJourneyView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
            ViewModel.IsConnected = App.Self.IsConnected;
            LoadData();
            id = ViewModel.SystemUser.IsAuthenticated == 3 ? ViewModel.ClinicianUser.HCPID : ViewModel.SystemUser.ICANN;
            btnWebview.IsVisible = !string.IsNullOrEmpty(id);
            btnWebview.Clicked += delegate
            {
                var which = btnWebview.Text == Langs.Launch_Ican;
                if (which)
                {
                    Launcher().ConfigureAwait(true);
                }
            };
        }

        async Task Launcher()
        {
            await DisplayAlert(Langs.ICan_Title, string.Format("{0} {1}. {2}",
                                                               ViewModel.SystemUser.IsAuthenticated == 3 ? Langs.ICan_Message1 : Langs.ICan_Message3,
                                                               id, Langs.ICan_Message2), Langs.Gen_OK).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                    Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri("https://apps.nelft.nhs.uk/ican/")));
            });
        }

        void LoadData()
        {
            ViewModel.ClientID = ViewModel.SystemUser.RIOID;
            ViewModel.HCP = ViewModel.ClinicianUser.HCPID;
            appointments = ViewModel.Appointments;

            if (appointments != null)
            {
                var sortedList = appointments.OrderByDescending(o => o.AppointmentDateTime.CleanDate()).ToList();

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

                txtApptsWeek.Text = thisWeek.ToString();
                txtApptsMonth.Text = thisMonth.ToString();

                ClientAppointmentsListView.ItemsSource = sortedList;
                ClientAppointmentsListView.IsPullToRefreshEnabled = true;
                ClientAppointmentsListView.Refreshing += (object sender, EventArgs e) =>
                 {
                     ClientAppointmentsListView.IsRefreshing = true;
                     ViewModel.ClientID = ViewModel.SystemUser.RIOID;
                     ViewModel.HCP = ViewModel.ClinicianUser.HCPID;
                     appointments = ViewModel.Appointments;
                     Device.BeginInvokeOnMainThread(() =>
                         {
                             ClientAppointmentsListView.ItemsSource = null;
                             ClientAppointmentsListView.ItemsSource = appointments;
                         });
                     ClientAppointmentsListView.IsRefreshing = false;
                 };
                ClientAppointmentsListView.ItemSelected += (sender, e) =>
                {
                    if (e.SelectedItem == null) return; // don't do anything if we just de-selected the row

                    var appointment = e.SelectedItem as Appointment;

                    //await DisplayAlert(Langs.MyJourney_AlertSelected, string.Format("{0} {1}", Langs.MyJourney_AlertSelectedMsg, appointment.AppointmentDateTime), Langs.Gen_OK);
                    ((ListView)sender).SelectedItem = null; // de-select the row
                };
            }
        }

        void Handle_Clicked(object s, EventArgs e)
        {
            // do nothing
        }


    }
}
