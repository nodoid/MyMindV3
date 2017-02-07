using System;

using Xamarin.Forms;

namespace MyMindV3
{
    public class MyWebview : ContentPage
    {
        public MyWebview()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

