using System;
using Xamarin.Forms;
using MyMindV3.Languages;
namespace MyMindV3
{
    public class ResourcesView : ViewCell
    {
        public ResourcesView()
        {
            var lblCategory = new Label
            {
                Text = "foo",
                TextColor = Color.Black,
                FontSize = 15
            };
            lblCategory.SetBinding(Label.TextProperty, new Binding("ResourceCategory"));

            var lblLocation = new Label
            {
                Text = "foo",
                TextColor = Color.Black,
                FontSize = 15
            };
            lblLocation.SetBinding(Label.TextProperty, new Binding("ResourceLocation"));

            var lblAddress = new Label
            {
                Text = "foo",
                TextColor = Color.Black,
                FontSize = 15,
                VerticalTextAlignment = TextAlignment.Center
            };
            lblAddress.SetBinding(Label.TextProperty, new Binding("ResourceAddress"));

            var btnAddressUrl = new Button
            {
                Text = "Url",
                HeightRequest = 36
            };

            btnAddressUrl.Clicked += (sender, e) =>
            {
                var url = ((Button)sender).ClassId;
                if (!string.IsNullOrEmpty(url))
                    Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri(url)));
            };

            lblAddress.BindingContextChanged += (sender, e) =>
            {
                var txt = ((Label)sender).Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    if (txt.Contains("="))
                    {
                        btnAddressUrl.IsEnabled = true;
                        var textIdx = txt.IndexOf('>');
                        var textOne = txt.Substring(textIdx + 1);
                        var textIdx2 = textOne.IndexOf('<');
                        btnAddressUrl.ClassId = lblAddress.Text = textOne.Substring(0, textIdx2);
                    }
                    else
                        btnAddressUrl.IsEnabled = false;
                }
            };

            var lblReferrals = new Label
            {
                Text = "foo",
                TextColor = Color.Black,
                FontSize = 15
            };
            lblReferrals.SetBinding(Label.TextProperty, new Binding("ResourceReferrals"));

            var lblContact = new Label
            {
                Text = "foo",
                TextColor = Color.Black,
                FontSize = 15,
                VerticalTextAlignment = TextAlignment.Center
            };
            lblContact.SetBinding(Label.TextProperty, new Binding("ResourceContactInfo"));

            var btnContact = new Button
            {
                Text = "Url",
                HeightRequest = 36,
            };

            btnContact.Clicked += (sender, e) =>
            {
                var url = ((Button)sender).ClassId;
                if (!string.IsNullOrEmpty(url))
                    Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri(url)));
            };

            lblContact.BindingContextChanged += (sender, e) =>
            {
                var txt = ((Label)sender).Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    if (txt.Contains("="))
                    {
                        btnContact.IsEnabled = true;
                        var textIdx = txt.IndexOf('>');
                        var textOne = txt.Substring(textIdx + 1);
                        var textIdx2 = textOne.IndexOf('<');
                        btnContact.ClassId = lblContact.Text = textOne.Substring(0, textIdx2);
                    }
                    else
                        btnContact.IsEnabled = false;
                }
            };

            View = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(20),
                Spacing = 5,
                BackgroundColor = Color.White,
                Children =
                {
                    new Label
                    {
                        Text = Langs.MyResources_Category,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.Black
                    },
                    lblCategory,
                    new Label
                    {
                        Text = Langs.MyResources_Location,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.Black
                    },
                    lblLocation,
                    new Label
                    {
                        Text = Langs.MyResources_Address,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.Black
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children = {btnAddressUrl, lblAddress}
                    },
                    new Label
                    {
                        Text = Langs.MyResources_Referral,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.Black
                    },
                    lblReferrals,
                    new Label
                    {
                        Text = Langs.MyResources_Contact,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.Black
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children = {btnContact, lblContact}
                    }
                }
            };
        }
    }
}
