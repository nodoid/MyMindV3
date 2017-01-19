
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class MyChatView : ContentPage
    {
        public MyChatView(string guid)
        {
            InitializeComponent();
            var deskey = "yxO7qG5CK03M1fJ7fP6cuXwxz3H5t9pI";
            var enc = DependencyService.Get<IEncrypt>().EncryptString(guid, deskey);
            var iv = DependencyService.Get<IEncrypt>().Iv_To_Pass_To_Encryption;

            webView.Source = string.Format("https://apps.nelft.nhs.uk/cometchat/index.php?a={0}&b={1}", enc, iv);
        }
    }
}
