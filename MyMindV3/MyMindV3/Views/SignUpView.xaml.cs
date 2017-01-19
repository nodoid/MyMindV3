using MvvmFramework.ViewModel;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class SignUpView : ContentPage
    {
        SignUpViewModel ViewModel => App.Locator.SignUp;

        public SignUpView()
        {
            InitializeComponent();

            BindingContext = ViewModel;
        }
    }
}
