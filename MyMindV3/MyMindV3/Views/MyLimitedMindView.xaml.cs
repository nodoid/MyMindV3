using MvvmFramework.ViewModel;
using MyMindV3.Languages;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class MyLimitedMindView : ContentPage
    {
        MyLimitedMindViewModel ViewModel => App.Locator.MyLimitedMind;

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        public MyLimitedMindView()
        {
            InitializeComponent();
            BindingContext = ViewModel;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                UpdateSessionTimeOut();
            };

            NavigationPage.SetHasBackButton(this, false);
            StackLayoutCover.GestureRecognizers.Add(tapGestureRecognizer);

            ToolbarItems.Add(new ToolbarItem(Langs.MyMind_Logout, "iconlogout", () =>
{
    ViewModel.Logout();
    ViewModel.LogoutCommand.Execute(null);
    Navigation.PopToRootAsync();
}));
        }

        void UpdateSessionTimeOut()
        {
            ViewModel.UpdateSessionExpirationTime();
        }
    }
}
