
using MvvmFramework;
using MvvmFramework.ViewModel;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class MyChatView : ContentPage
    {
        MyChatViewModel ViewModel => App.Locator.MyChat;

        CustomChatView chatView;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "UserId")
                {
                    chatView.Username = ViewModel.UserId;
                }
            };
        }

        public MyChatView(string guid)
        {
            InitializeComponent();
            BindingContext = ViewModel;
            var enc = App.Self.Encrypt.EncryptString(guid, Constants.DESKey);
            var iv = App.Self.Encrypt.Iv_To_Pass_To_Encryption;

            webView.Source = string.Format("{0}/index.php?a={1}&b={2}", Constants.CometChatURL, enc, iv);
            webView.Source = "https://apps.nelft.nhs.uk/cometchat/cometchat_popout.php?type=plugin&name=avchat&subtype=webrtc&embed=";
            /*ViewModel.GetUserId(guid);

            chatView = new CustomChatView
            {
                WidthRequest = App.ScreenSize.Width,
                HeightRequest = App.ScreenSize.Height,
            };
            chatView.SetBinding(CustomChatView.UsernameProperty, new Binding("UserId"));*/
            Content = new StackLayout
            {
                Children = { /*chatView*/ webView }
            };
        }
    }
}
