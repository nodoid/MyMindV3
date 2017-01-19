using MvvmFramework.ViewModel;
using Xamarin.Forms;

namespace MyMindV3
{
    public partial class MyFileView : ContentPage
    {
        MyJourneyViewModel ViewModel => App.Locator.MyJourney;

        public MyFileView(string url)
        {
            InitializeComponent();
            webView.Source = new UrlWebViewSource { Url = string.Format("file:///{0}", url) };
        }
    }
}
