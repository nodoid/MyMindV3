using System;
using Xamarin.Forms;
using MyMindV3.Languages;
using MvvmFramework.ViewModel;

namespace MyMindV3.Views
{
    public partial class MyMindView : ContentPage
    {
        MyMindViewModel ViewModel => App.Locator.MyMind;

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        public MyMindView()
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

            if (ViewModel.SystemUser.IsAuthenticated == 3)
                btnPatient.Text = Langs.MyMind_PatientProfile;
        }

        void LoadMyProfileBtn(object sender, EventArgs e)
        {
            if (ViewModel.SystemUser.IsAuthenticated != 3)
                ViewModel.ShowMyProfileCommand.Execute(null);
            else
                ViewModel.ShowMyPatientCommand.Execute(null);
        }

        void LoadMyHelpBtn(object sender, EventArgs e)
        {
            ViewModel.ShowMyUserHelpCommand.Execute(null);
        }

        void UpdateSessionTimeOut()
        {
            ViewModel.UpdateSessionExpirationTime();
        }
    }
}
