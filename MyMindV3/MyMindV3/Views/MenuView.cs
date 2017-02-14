using Xamarin.Forms;
using System.Collections.Generic;
using MyMindV3.Languages;
using System;
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

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        StackLayout GenerateUI(List<string> filenames)
        {
            var fns = filenames.Distinct().ToList();
            menuList = new List<MenuListClass>();
            foreach (var f in fns)
            {
                var idsplit = f.Split('|');
                var file = idsplit[0].Replace("_", " ").Split(' ');
                var ourname = string.Empty;
                if (file.Length != 1)
                {
                    ourname = string.Format("{0} {1}", UppercaseFirst(file[0]), file[1]);
                }
                else
                    ourname = UppercaseFirst(file[0]);

                menuList.Add(new MenuListClass { text = ourname, image = idsplit[0], id = Convert.ToInt32(idsplit[1]) });
            }

            var innerStack = new StackLayout
            {
                BackgroundColor = Color.White,
                Orientation = StackOrientation.Vertical,
            };

            for (var n = 0; n < menuList.Count; ++n)
                innerStack.Children.Add(MenuListView(n));

            var stackScroll = new ScrollView
            {
                WidthRequest = App.ScreenSize.Width * .3,
                TranslationY = -48,
                Content = innerStack
            };

            var lblResource = new Label
            {
                Text = Langs.Menu_Resources,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var radio = new BindableRadioGroup
            {
                ItemsSource = new List<string> { Langs.MyResources_Distance, Langs.Menu_AveRating, Langs.Menu_AZ, Langs.Menu_Popular },
                TranslationY = -8,
            };

            radio.SelectedIndex = SetSelected;

            radio.CheckedChanged += (sender, e) =>
            {
                var button = sender as CustomRadioButton;
                MessagingCenter.Send(this, "buttonClicked", button.Id);
            };

            var topStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    lblResource,
                    radio,
                    new BoxView
                    {
                        TranslationY = -56,
                        Color = Color.Gray,
                        HeightRequest = 1
                    }
                }
            };

            topStack.SizeChanged += (sender, e) =>
            {
                stackScroll.HeightRequest = App.ScreenSize.Height - topStack.Height;
            };

            var masterStack = new StackLayout
            {
                BackgroundColor = Color.White,
                Orientation = StackOrientation.Vertical,
                MinimumWidthRequest = App.ScreenSize.Width * .35,
                WidthRequest = App.ScreenSize.Width * .35,
                HeightRequest = App.ScreenSize.Height /*- 52 - 36*/,
                TranslationY = 8,
                Spacing = 0,
                Padding = new Thickness(0),
                StyleId = "menu",
                Children = { topStack, stackScroll }
            };

            return masterStack;
        }

        public MenuView(List<string> filenames, int selected)
        {
            SetSelected = selected;

            Content = GenerateUI(filenames);
        }

        public void UpdateMenu(List<string> filenames, int selected)
        {
            SetSelected = selected;
            if (Content != null)
                Content = null;
            Content = GenerateUI(filenames);
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


