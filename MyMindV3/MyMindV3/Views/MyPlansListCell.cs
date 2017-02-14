using MvvmFramework.Models;
using Xamarin.Forms;
using MyMindV3.Languages;

namespace MyMindV3
{
    public class MyPlansListCell : ViewCell
    {
        public MyPlansListCell()
        {
            var imgFiletype = new Image
            {
                HeightRequest = 48,
                WidthRequest = 48,
            };

            var lblFilename = new Label
            {
                FontSize = 15,
                TextColor = Color.White
            };
            lblFilename.SetBinding(Label.TextProperty, new Binding("FileName"));

            var lblUploaded = new Label
            {
                FontSize = 15,
                TextColor = Color.White
            };
            this.BindingContextChanged += (sender, e) =>
            {
                if (BindingContext != null)
                {
                    var ts = BindingContext as ClientPlan;
                    var lc = ts.FileType.ToLowerInvariant();
                    if (lc.Contains("jpg") || lc.Contains("jpeg"))
                        imgFiletype.Source = "jpg";
                    if (lc.Contains("png"))
                        imgFiletype.Source = "png";
                    if (lc.Contains("pdf"))
                        imgFiletype.Source = "pdf";
                    if (lc.Contains("doc"))
                        imgFiletype.Source = "doc";

                    var dt = ts.FileUploadDateTime.Split('T');
                    var t = dt[1].Split('.');
                    lblUploaded.Text = string.Format("{0}\n{1}", dt[0], t[0]);
                }
            };
            var btnView = new Button
            {
                Text = Langs.MyMind_PlansView
            };
            btnView.SetBinding(Button.ClassIdProperty, new Binding("FileID"));

            var stack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                WidthRequest = App.ScreenSize.Width * .95,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                Children =
                {
                    new StackLayout
                    {
                        WidthRequest = App.ScreenSize.Width * .2,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Children = {imgFiletype}
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        WidthRequest = App.ScreenSize.Width * .8,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(4),
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new Label {Text = Langs.Login_Name, FontSize = 15, TextColor = Color.White, FontAttributes = FontAttributes.Bold},lblFilename
                                }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new Label {Text = Langs.MyMind_PlansUpload, FontSize = 15, TextColor = Color.White, FontAttributes = FontAttributes.Bold}, lblUploaded
                                }
                            }
                        }
                    }
                }
            };
            stack.SetBinding(View.ClassIdProperty, new Binding("FileID"));
            View = stack;
        }
    }
}

