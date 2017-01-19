using MyMindV3.Classes;
using MyMindV3.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MyMindV3.Languages;
using MvvmFramework.ViewModel;
using MvvmFramework;

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
            LoadData().ConfigureAwait(true);
            id = RootVM.SystemUser.IsAuthenticated == 3 ? ViewModel.ClinicianUser.HCPID : ViewModel.SystemUser.ICANN;
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

        private async Task LoadData()
        {
            appointments = await GetPatientAppointments(RootVM.SystemUser.RIOID, RootVM.ClinicianUser.HCPID);

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
            ClientAppointmentsListView.Refreshing += async (object sender, EventArgs e) =>
            {
                ClientAppointmentsListView.IsRefreshing = true;
                await GetPatientAppointments(RootVM.SystemUser.RIOID, RootVM.ClinicianUser.HCPID).ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                            {
                                ClientAppointmentsListView.ItemsSource = null;
                                ClientAppointmentsListView.ItemsSource = t.Result;
                            });
                        appointments = t.Result;
                        ClientAppointmentsListView.IsRefreshing = false;
                    }
                });
            };
            ClientAppointmentsListView.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null) return; // don't do anything if we just de-selected the row

                var appointment = e.SelectedItem as Classes.Appointment;

                //await DisplayAlert(Langs.MyJourney_AlertSelected, string.Format("{0} {1}", Langs.MyJourney_AlertSelectedMsg, appointment.AppointmentDateTime), Langs.Gen_OK);
                ((ListView)sender).SelectedItem = null; // de-select the row
            };
        }

        void Handle_Clicked(object s, EventArgs e)
        {
            // do nothing
        }

        private async Task<IEnumerable<Appointment>> GetPatientAppointments(string clientId, string hcp)
        {
            //string url = string.Format("https://apps.nelft.nhs.uk/CareMapApi/api/Appointment/GetAppointmentsByClient?clientId={0}&startDate={1}&endDate={2}", 
            //clientId, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            var url = string.Format("{0}/api/Appointment/GetAppointmentsByClientAndHCP?clientId={1}&hcpCode={2}",
                                    Constants.BaseTestUrl, clientId, hcp);

            IEncryptionManager encMgr = Factory.Instance.GetEncryptionManager();

            var client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(url);
            var encryptionJson = response.Content.ReadAsStringAsync().Result;
            var encryptions = JsonConvert.DeserializeObject<IEnumerable<Encryption>>(encryptionJson);

            return encMgr.DecryptAppointments(encryptions);
        }
    }
}
