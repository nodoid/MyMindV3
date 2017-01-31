using MvvmFramework.ViewModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using MyMindV3.Languages;

namespace MyMindV3.Views
{
    public partial class LoginView : ContentPage
    {
        LoginViewModel ViewModel => App.Locator.Login;
        private int _imgCount = 1;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsBusy")
                {
                    Device.BeginInvokeOnMainThread(() => DependencyService.Get<INetworkSpinner>().NetworkSpinner(ViewModel.IsBusy, ViewModel.SpinnerTitle, ViewModel.SpinnerMessage));
                }
            };
        }

        // construct
        public LoginView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
            ViewModel.IsConnected = App.Self.IsConnected;
            InitBGTimer().ConfigureAwait(true);

            var errorMessage = new Dictionary<string, string>
            {
                {"Login_InvalidError", Langs.Login_InvalidError}
            };
            var errorTitle = new Dictionary<string, string>
            {
                {"Gen_Error", Langs.Gen_Error}
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
            await Task.Delay(5000);
            SwapImage();
        }

        void SwapImage()
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

    }
}
