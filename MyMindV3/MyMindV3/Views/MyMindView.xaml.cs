using MyMindV3.ViewModels;
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
            //NavigationPage.SetTitleIcon(this, "iconlogout");

            /* add tapgesture recognition to named stacklayout */
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                UpdateSessionTimeOut();
            };
            StackLayoutCover.GestureRecognizers.Add(tapGestureRecognizer);

            ToolbarItems.Add(new ToolbarItem(Langs.MyMind_Logout, "iconlogout", async () =>
{

    await Navigation.PushAsync(new LoginView());
    Navigation.RemovePage(this);
    ViewModel.Logout();
}));

            if (ViewModel.SystemUser.IsAuthenticated == 3)
                btnPatient.Text = Langs.MyMind_PatientProfile;
        }


        private async void LoadMyProfileBtn(object sender, EventArgs e)
        {
            if (ViewModel.SystemUser.IsAuthenticated != 3)
                await Navigation.PushAsync(new MyProfileView(), true);
            else
                await Navigation.PushAsync(new MyPatient(), true);
        }

        private async void LoadClinicianBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyClinicianView(), true);
        }

        private async void LoadJourneyBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyJourneyView(), true);
        }

        private async void LoadChatBtn(object sender, EventArgs e)
        {
            var guid = RootVM.SystemUser.IsAuthenticated == 3 ? RootVM.ClinicianUser.ClinicianGUID : RootVM.SystemUser.Guid;

            await Navigation.PushAsync(new MyChatView(guid), true);
        }

        private async void LoadPlansBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyPlansView(), true);
        }

        private async void LoadResourcesBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyResourcesView(), true);
        }

        void UpdateSessionTimeOut()
        {
            ViewModel.UpdateSessionExpirationTime();
        }
    }
}
