using System;
using Xamarin.Forms;
using MyMindV3.Languages;
using MvvmFramework.ViewModel;

namespace MyMindV3.Views
{
    public partial class MyMindView : ContentPage
    {
        MyMindViewModel ViewModel => App.Locator.MyMind;

        public MyMindView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
            ViewModel.IsConnected = App.Self.IsConnected;
            //NavigationPage.SetTitleIcon(this, "iconlogout");

            /* add tapgesture recognition to named stacklayout */
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                UpdateSessionTimeOut();
            };
            StackLayoutCover.GestureRecognizers.Add(tapGestureRecognizer);

            ToolbarItems.Add(new ToolbarItem(Langs.MyMind_Logout, "iconlogout", () =>
{
    ViewModel.LogoutCommand.Execute(null);
    Navigation.RemovePage(this);
    ViewModel.Logout();
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

        void UpdateSessionTimeOut()
        {
            ViewModel.UpdateSessionExpirationTime();
        }
    }
}
