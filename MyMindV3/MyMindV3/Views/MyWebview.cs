using Xamarin.Forms;

namespace MyMindV3
{
    public class MyWebview : ContentPage
    {
        public MyWebview(string uri)
        {
            Content = new StackLayout
            {
                HeightRequest = App.ScreenSize.Height,
                WidthRequest = App.ScreenSize.Width,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                Children =
                {
                    new WebView {Source = uri}
                }
            };
        }
    }
}

