using Xamarin.Forms;
using System.Collections.Generic;
using MyMindV3.Languages;
using System.Linq;

namespace MyMindV3.Views
{
    public class MenuListClass
    {
        public string image { get; set; }

        public string text { get; set; }

        public bool enabled { get; set; } = true;

        public int id { get; set; }
    }

    public class MenuView : ContentView
    {
        List<MenuListClass> menuList;

        int SetSelected { get; set; }
        int SetCategory { get; set; }
        List<string> Categories { get; set; }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        StackLayout GenerateUI()
        {
            var lblResource = new Label
            {
                Text = Langs.MyResources_Filter,
                FontSize = 14,
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var radio = new BindableRadioGroup
            {
                ItemsSource = new List<string> { Langs.MyResources_Distance, Langs.Menu_AveRating, Langs.Menu_AZ, Langs.Menu_Popular },
                TextColor = Color.White,
                FontSize = 14,
                TranslationY = Device.OS == TargetPlatform.Android ? -8 : 0
            };

            radio.SelectedIndex = SetSelected;

            radio.CheckedChanged += (sender, e) =>
            {
                var button = sender as CustomRadioButton;
                MessagingCenter.Send(this, "buttonClicked", button.Id);
            };

            var catStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
            };
            if (Categories.Count != 0)
            {
                var catScroll = new ScrollView
                {
                    HeightRequest = App.ScreenSize.Height * .55
                };
                var catlist = new BindableRadioGroup
                {
                    ItemsSource = Categories,
                    SelectedIndex = SetCategory
                };
                Device.OnPlatform(Android: (() => catlist.TranslationY = -8));
                catlist.CheckedChanged += (sender, e) =>
                {
                    var button = sender as CustomRadioButton;
                    MessagingCenter.Send(this, "catButtonClicked", button.Id);
                };
                catScroll.Content = catlist;
                catStack.Children.Add(catScroll);
            }

            var topStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    lblResource,
                    radio,
                    new BoxView
                    {
                        Color = Color.Gray,
                        HeightRequest = 1
                    },
                    new Label
                    {
                        Text =Langs.MyResources_Categories,
                        TextColor = Color.White,
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                }
            };

            var masterStack = new StackLayout
            {
                BackgroundColor = Color.FromHex("022330"),
                Opacity = .8,
                Orientation = StackOrientation.Vertical,
                MinimumWidthRequest = App.ScreenSize.Width * .35,
                WidthRequest = App.ScreenSize.Width * .35,
                HeightRequest = App.ScreenSize.Height /*- 52 - 36*/,
                TranslationY = 8,
                Spacing = 0,
                Padding = new Thickness(0),
                StyleId = "menu",
                Children =
                {
                    topStack,
                    catStack
                }
            };

            return masterStack;
        }

        public MenuView(int selected, int cat, List<string> cats)
        {
            SetSelected = selected;
            SetCategory = cat;
            Categories = cats.OrderBy(x => x).ToList();
            Content = GenerateUI();
        }

        public void UpdateMenu(int selected, int cat, List<string> cats)
        {
            SetSelected = selected;
            SetCategory = cat;
            Categories = cats.OrderBy(x => x).ToList();
            if (Content != null)
                Content = null;
            Content = GenerateUI();
        }

        StackLayout MenuListView(int i)
        {
            var imgIcon = new Image
            {
                WidthRequest = 36,
                HeightRequest = 36,
                Source = menuList[i].image
            };

            var lblText = new Label
            {
                FontSize = 12,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Blue,
                Text = menuList[i].text
            };

            var stackTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() => MessagingCenter.Send(this, "IconTap", menuList[i].id))
            };

            var stack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(0),
                Spacing = 0,
                Children =
                {
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Padding = new Thickness(8,0,0,4),
                        Children =
                        { imgIcon,
                            new StackLayout
                            {
                                VerticalOptions = LayoutOptions.Center,
                                Children = { lblText }
                            }
                        }
                    },
                    new BoxView
                    {
                        WidthRequest = App.ScreenSize.Width,
                        HeightRequest = 1,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Color = Color.Gray
                    }
                },
            };
            stack.GestureRecognizers.Add(stackTap);

            return stack;
        }


    }
}


