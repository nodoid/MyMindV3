using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MyMindV3.Languages;
using MvvmFramework.ViewModel;
using MvvmFramework.Models;
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
            ViewModel.IsConnected = App.Self.IsConnected;
            LoadData();
            id = ViewModel.SystemUser.IsAuthenticated == 3 ? ViewModel.ClinicianUser.HCPID : ViewModel.SystemUser.ICANN;
            AddUIToContent();
        }

        void AddUIToContent()
        {
            var btnWebview = new Button
            {
                Text = Langs.Launch_Ican,
                IsVisible = !string.IsNullOrEmpty(id)
            };
            btnWebview.Clicked += delegate
            {
                var which = btnWebview.Text == Langs.Launch_Ican;
                if (which)
                {
                    Launcher().ConfigureAwait(true);
                }
            };
            ContentStack.Children.Add(btnWebview);

            var masterGrid = new Grid
            {
                WidthRequest = App.ScreenSize.Width,
                HeightRequest = App.ScreenSize.Height,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition {Width = App.ScreenSize.Width * .3},
                    new ColumnDefinition {Width = App.ScreenSize.Width * .3},
                    new ColumnDefinition {Width = App.ScreenSize.Width * .3}
                },
                ColumnSpacing = 2,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition {Height = 22},
                    new RowDefinition {Height = GridLength.Star}
                }
            };

            var lblTop = new Label
            {
                Text = Langs.MyJourney_PastAppts,
                TextColor = Color.White,
                BackgroundColor = Color.Red,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            var lblNext = new Label
            {
                Text = Langs.MyJourney_ComingAppts,
                TextColor = Color.Red,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var stack = new StackLayout
            {
                WidthRequest = App.ScreenSize.Width,
                Orientation = StackOrientation.Vertical,
                Children = { SwapView(0) }
            };

            var lblTopTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        stack.Children.RemoveAt(0);
                        stack.Children.Add(SwapView(0));
                        lblTop.BackgroundColor = Color.Red;
                        lblNext.BackgroundColor = Color.White;
                        lblTop.TextColor = Color.White;
                        lblNext.TextColor = Color.Red;
                    });
                })
            };

            var lblNextTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        stack.Children.RemoveAt(0);
                        stack.Children.Add(SwapView(1));
                        lblTop.BackgroundColor = Color.White;
                        lblNext.BackgroundColor = Color.Red;
                        lblTop.TextColor = Color.Red;
                        lblNext.TextColor = Color.White;
                    });
                })
            };

            lblTop.GestureRecognizers.Add(lblTopTap);
            lblNext.GestureRecognizers.Add(lblNextTap);

            masterGrid.Children.Add(lblTop, 0, 0);
            masterGrid.Children.Add(lblNext, 1, 0);
            masterGrid.Children.Add(stack, 0, 1);
            Grid.SetColumnSpan(stack, 3);

            ContentStack.Children.Add(masterGrid);
        }

        StackLayout SwapView(int view)
        {
            var listPast = new ListView
            {
                HasUnevenRows = true,
                ItemsSource = view == 0 ? ViewModel.PastAppointments : ViewModel.NextAppointments,
                ItemTemplate = new DataTemplate(typeof(ApptList)),
                IsPullToRefreshEnabled = true,
            };

            listPast.Refreshing += (object sender, EventArgs e) =>
             {
                 listPast.IsRefreshing = true;
                 ViewModel.ClientID = ViewModel.SystemUser.RIOID;
                 ViewModel.HCP = ViewModel.ClinicianUser.HCPID;
                 appointments = view == 0 ? ViewModel.PastAppointments : ViewModel.NextAppointments;
                 Device.BeginInvokeOnMainThread(() =>
                     {
                         listPast.ItemsSource = null;
                         listPast.ItemsSource = appointments;
                     });
                 listPast.IsRefreshing = false;
             };
            listPast.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null) return; // don't do anything if we just de-selected the row

                var appointment = e.SelectedItem as Appointment;

                //await DisplayAlert(Langs.MyJourney_AlertSelected, string.Format("{0} {1}", Langs.MyJourney_AlertSelectedMsg, appointment.AppointmentDateTime), Langs.Gen_OK);
                ((ListView)sender).SelectedItem = null; // de-select the row
            };

            return new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(8, 8),
                Children = { listPast }
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
            ViewModel.GetAppointments();
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
            }
        }

        void Handle_Clicked(object s, EventArgs e)
        {
            // do nothing
        }


    }
}
