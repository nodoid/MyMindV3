using MyMindV3.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class RootView : ContentPage
    {
        private RootViewModel _rootVM;

        public RootView()
        {
            InitializeComponent();
            _rootVM = new RootViewModel();
            BindingContext = _rootVM;

            InitRedirectTimer();
        }

        public async void InitRedirectTimer()
        {
            await Task.Delay(100);
            DisplayLogin();
        }

        void DisplayLogin()
        {
            App.Current.MainPage = new NavigationPage(new LoginView(_rootVM));
        }

    }
}
