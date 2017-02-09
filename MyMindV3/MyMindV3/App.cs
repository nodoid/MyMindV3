﻿using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using MvvmFramework;
using MyMindV3.Languages;
using MyMindV3.Views;
using Plugin.Connectivity;
using System.Globalization;

using Xamarin.Forms;
using Plugin.Geolocator.Abstractions;
using Plugin.Geolocator;

#if DEBUG
using System.Diagnostics;
#endif

namespace MyMindV3
{
    public class App : Application
    {
        public static Size ScreenSize { get; set; }
        public string ContentDirectory { get; private set; }
        public string PicturesDirectory { get; private set; } = DependencyService.Get<IContent>().PicturesDirectory();

        public IUserSettings UserSettings { get; set; } = DependencyService.Get<IUserSettings>();
        public static ViewModelLocator locator;
        public static ViewModelLocator Locator { get { return locator ?? (locator = new ViewModelLocator()); } }

        public static App Self { get; private set; }

        public bool IsConnected { get; private set; }

        Position location;
        public Position Location
        {
            get { return location; }
            set
            {
                location = value;
                OnPropertyChanged("Location");
            }
        }

        int newIconRating;
        public int NewIconRating
        {
            get { return newIconRating; }
            set
            {
                newIconRating = value;
                OnPropertyChanged("NewIconRating");
            }
        }

        public bool PanelShowing { get; set; }

        public int IdInUse { get; set; }

        public App()
        {
            App.Self = this;

            var netLanguage = DependencyService.Get<ILocalize>().GetCurrent();
            Langs.Culture = new CultureInfo(netLanguage);
            DependencyService.Get<ILocalize>().SetLocale();

            CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
            {
                IsConnected = args.IsConnected;
            };

            IsConnected = CrossConnectivity.Current.IsConnected;

            CrossGeolocator.Current.StartListeningAsync(3000, 10, true);
            if (CrossGeolocator.Current.IsListening)
                CrossGeolocator.Current.GetPositionAsync(3000).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                    Location = t.Result;
            });

            CrossGeolocator.Current.PositionError += (object sender, PositionErrorEventArgs e) =>
            {
#if DEBUG
                Debug.WriteLine(e.Error.ToString());
#endif
            };

            CrossGeolocator.Current.PositionChanged += (object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e) =>
            {
                Location = e.Position;
            };

            var nav = new NavigationService();
            nav.Configure(ViewModelLocator.AllRegisteredUsersKey, typeof(AllRegisteredUsers));
            nav.Configure(ViewModelLocator.LoginKey, typeof(LoginView));
            nav.Configure(ViewModelLocator.LoginPinCodeKey, typeof(LoginPincodeView));
            nav.Configure(ViewModelLocator.MyChatKey, typeof(MyChatView));
            nav.Configure(ViewModelLocator.MyClinicianKey, typeof(MyClinicianView));
            nav.Configure(ViewModelLocator.MyFileKey, typeof(MyFileView));
            nav.Configure(ViewModelLocator.MyJourneyKey, typeof(MyJourneyView));
            nav.Configure(ViewModelLocator.MyLimitedMindKey, typeof(MyLimitedMindView));
            nav.Configure(ViewModelLocator.MyMindKey, typeof(MyMindView));
            nav.Configure(ViewModelLocator.MyPatientKey, typeof(MyPatient));
            nav.Configure(ViewModelLocator.MyPlansKey, typeof(MyPlansView));
            nav.Configure(ViewModelLocator.MyProfileKey, typeof(MyProfileView));
            nav.Configure(ViewModelLocator.MyResourcesKey, typeof(MyResourcesView));
            nav.Configure(ViewModelLocator.SignupKey, typeof(SignUpView));
            nav.Configure(ViewModelLocator.MyHelpKey, typeof(MyHelpView));
            SimpleIoc.Default.Register<INavigationService>(() => nav);

            // Register the SQL
            DependencyService.Get<MvvmFramework.ISqLiteConnectionFactory>().GetConnection();
            var dialogService = new DialogService();
            SimpleIoc.Default.Register<IDialogService>(() => dialogService);
            // we next set the navigation page
            var firstPage = new NavigationPage(new LoginView());

            // initialise the navigation service with the page in firstPage
            nav.Initialize(firstPage);

            dialogService.Initialize(firstPage);

            // and launch

            MainPage = firstPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
