using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class RootView : ContentPage
    {
        public RootView()
        {
            InitializeComponent();
            InitRedirectTimer();
        }

        public async void InitRedirectTimer()
        {
            await Task.Delay(100);
            DisplayLogin();
        }

        void DisplayLogin()
        {
            App.Current.MainPage = new NavigationPage(new LoginView());
        }

    }
}
