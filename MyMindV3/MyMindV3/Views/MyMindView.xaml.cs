using MyMindV3.ViewModels;
using System;
using Xamarin.Forms;
using MyMindV3.Languages;

namespace MyMindV3.Views
{
    public partial class MyMindView : ContentPage
    {
        private MyMindViewModel _myMindVM;
        private RootViewModel _rootVM;

        public MyMindView(RootViewModel rootVM)
        {
            InitializeComponent();
            RootVM = rootVM;
            _myMindVM = new MyMindViewModel(_rootVM);
            BindingContext = _myMindVM;
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

    await Navigation.PushAsync(new LoginView(rootVM));
    Navigation.RemovePage(this);
    rootVM.Logout();
}));

            if (RootVM.SystemUser.IsAuthenticated == 3)
                btnPatient.Text = Langs.MyMind_PatientProfile;
        }


        private async void LoadMyProfileBtn(object sender, EventArgs e)
        {
            if (RootVM.SystemUser.IsAuthenticated != 3)
                await Navigation.PushAsync(new MyProfileView(RootVM), true);
            else
                await Navigation.PushAsync(new MyPatient(RootVM), true);
        }

        private async void LoadClinicianBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyClinicianView(RootVM), true);
        }

        private async void LoadJourneyBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyJourneyView(RootVM), true);
        }

        private async void LoadChatBtn(object sender, EventArgs e)
        {
            /* set the master/detail within App - to update detail */
            //App.MasterDetail = new ChatAll(_rootVM);
            //var rootMD = App.MasterDetail;

            //await Navigation.PushAsync(rootMD, true);

            var guid = RootVM.SystemUser.IsAuthenticated == 3 ? RootVM.ClinicianUser.ClinicianGUID : RootVM.SystemUser.Guid;

            await Navigation.PushAsync(new MyChatView(guid), true);
            //await Navigation.PushAsync(new CometChatView(), true);
        }

        private async void LoadPlansBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyPlansView(RootVM), true);
        }

        private async void LoadResourcesBtn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyResourcesView(), true);
        }

        void UpdateSessionTimeOut()
        {
            _rootVM.UpdateSessionExpirationTime();
        }


        // provide access to RootVM
        public RootViewModel RootVM
        {
            get { return _rootVM; }
            set
            {
                if (value != _rootVM)
                {
                    _rootVM = value;
                    OnPropertyChanged("RootVM");
                }
            }
        }

    }
}
