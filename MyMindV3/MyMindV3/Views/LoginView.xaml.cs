using MvvmFramework.ViewModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using MyMindV3.Languages;
using System;

namespace MyMindV3.Views
{
    public partial class LoginView : ContentPage
    {
        LoginViewModel ViewModel => App.Locator.Login;
        private int _imgCount = 1;
        bool stop;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            stop = false;
            ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsBusy")
                {
                    Task.Run(() => Device.BeginInvokeOnMainThread(() => DependencyService.Get<INetworkSpinner>().NetworkSpinner(ViewModel.IsBusy, ViewModel.SpinnerTitle, ViewModel.SpinnerMessage)));
                }
            };
            UserNameInput.Text = PasswordInput.Text = string.Empty;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            stop = true;
        }

        public LoginView()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = ViewModel;
            ViewModel.IsLoggedIn = ViewModel.DisplayAcceptance = false;
            ViewModel.DisplayLogin = true;
            UserNameInput.Text = PasswordInput.Text = string.Empty;

            InitBGTimer().ConfigureAwait(true);

            var errorMessage = new Dictionary<string, string>
            {
                {"Login_InvalidError", Langs.Login_InvalidError},
                {"Login_NetworkError", Langs.Login_NetworkError},
                {"Login_Locked", Langs.Login_NetworkLocked},
                {"Login_Error", Langs.Login_NetworkErrorReturned},
                {"Login_Verify", Langs.Login_NetworkVerification},
                {"Login_Fail", Langs.Login_NetworkGeneralFail}
            };
            var errorTitle = new Dictionary<string, string>
            {
                {"Gen_Error", Langs.Gen_Error},
                {"Error_NetworkTitle", Langs.Error_NetworkTitle},
                {"Login_ErrorTitle", Langs.Login_LoginFailTitle}
            };
            var message = new Dictionary<string, string>
            {
                {"LoggingIn_Title", Langs.LoggingIn_Title},
                {"Gen_PleaseWait", Langs.Gen_PleaseWait}
            };

            ViewModel.ErrorMessages = errorMessage;
            ViewModel.ErrorTitles = errorTitle;
            ViewModel.Messages = message;
        }

        public async Task InitBGTimer()
        {
            if (!ViewModel.IsLoggedIn && !stop)
            {
                await Task.Delay(5000);
                SwapImage();
            }
        }

        void SwapImage()
        {
            try
            {
                BackgrdImg.FadeTo(0);

                /* update counter & re-init timer */
                if (_imgCount < 13)
                    _imgCount++;
                else
                    _imgCount = 2;

                BackgrdImg.ImageSource = "bg" + _imgCount.ToString() + ".png";
                BackgrdImg.FadeTo(1);

                InitBGTimer().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // 
            }
        }

    }
}
