
using MvvmFramework;
using MvvmFramework.ViewModel;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class MyChatView : ContentPage
    {
        MyChatViewModel ViewModel => App.Locator.MyChat;

        public MyChatView(string guid)
        {
            InitializeComponent();
            BindingContext = ViewModel;
            var enc = App.Self.Encrypt.EncryptString(guid, Constants.DESKey);
            var iv = App.Self.Encrypt.Iv_To_Pass_To_Encryption;

            webView.Source = string.Format("{0}/index.php?a={1}&b={2}", Constants.CometChatURL, enc, iv);
        }
    }
}
