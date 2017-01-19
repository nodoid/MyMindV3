using MvvmFramework.ViewModel;
using MyMindV3.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class LoginView : ContentPage
    {
        LoginViewModel ViewModel => App.Locator.Login;
        private int _imgCount = 1;
        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";


        // construct
        public LoginView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
            InitBGTimer().ConfigureAwait(true);
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
