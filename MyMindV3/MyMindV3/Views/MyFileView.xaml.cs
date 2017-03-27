using System;
using MvvmFramework.ViewModel;
using Xamarin.Forms;

namespace MyMindV3
{
    public partial class MyFileView : ContentPage
    {
        MyFileViewModel ViewModel => App.Locator.MyFile;

        public MyFileView(string url)
        {
            InitializeComponent();
			var filePath = ViewModel.GetCurrentFolder;
			//var path = Device.OS == TargetPlatform.iOS ? filePath : string.Format("file://{0}/", filePath);

			var image = string.Format("{0}/{1}", filePath, url);
			/*vvar source = new HtmlWebViewSource
			{
				Html = "<html><body><img src=\"" + image + "\"/></body></html>",
				BaseUrl = path
			};*/
			webView.Source = new FileImageSource { File = image };
        }
    }
}
