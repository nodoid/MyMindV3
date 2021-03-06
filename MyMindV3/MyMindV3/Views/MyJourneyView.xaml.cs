﻿using System;
using System.Collections.Generic;
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
            if (!ViewModel.GetIsClinician)
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
                    new ColumnDefinition {Width = App.ScreenSize.Width * .33},
                    new ColumnDefinition {Width = App.ScreenSize.Width * .33},
                    new ColumnDefinition {Width = App.ScreenSize.Width * .33}
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
                BackgroundColor = Color.White,
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
                SeparatorVisibility = SeparatorVisibility.None,
                ItemsSource = view == 0 ? ViewModel.PastAppointments : ViewModel.NextAppointments,
                ItemTemplate = new DataTemplate(typeof(ApptList)),
                //IsPullToRefreshEnabled = true,
                BackgroundColor = Color.FromHex("022330")
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
            ViewModel.ClientID = App.Self.Encrypt.EncryptHcpString(ViewModel.SystemUser.RIOID);
            ViewModel.HCP = App.Self.Encrypt.EncryptHcpString(ViewModel.ClinicianUser.HCPID);
            ViewModel.GetAppointments();
            appointments = ViewModel.Appointments;

            if (appointments != null)
            {
                txtApptsWeek.Text = ViewModel.ApptsThisWeek;
                txtApptsMonth.Text = ViewModel.ApptsThisMonth;
            }
        }

        void Handle_Clicked(object s, EventArgs e)
        {
            // do nothing
        }
    }
}
