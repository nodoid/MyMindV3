using MyMindV3.ViewModels;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class SignUpView : ContentPage
    {
        private SignUpViewModel _signUpVM;
        private INavigation _navigation;

        public SignUpView(RootViewModel rootVM)
        {
            InitializeComponent();
            _navigation = this.Navigation;

            _signUpVM = new SignUpViewModel(rootVM, _navigation);
            BindingContext = _signUpVM;
        }
    }
}
