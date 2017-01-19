using Xamarin.Forms;

namespace MyMindV3
{
    public partial class MyFileView : ContentPage
    {
        public MyFileView(string url)
        {
            InitializeComponent();
            webView.Source = new UrlWebViewSource { Url = string.Format("file:///{0}", url) };
        }
    }
}
