using Xamarin.Forms;
using System.Collections.Generic;

namespace MyMindV3.Views
{
    public class MenuListClass
    {
        public string image { get; set; }

        public string text { get; set; }

        public bool enabled { get; set; } = true;
    }

    public class MenuView : ContentView
    {
        List<MenuListClass> menuList;

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public MenuView(List<string> filenames)
        {
            menuList = new List<MenuListClass>();
            foreach (var f in filenames)
            {
                var file = f.Replace("_", " ").Split(' ');
                var ourname = string.Empty;
                if (file.Length != 1)
                {
                    ourname = string.Format("{0} {1}", UppercaseFirst(file[0]), file[1]);
                }
                else
                    ourname = UppercaseFirst(file[0]);

                menuList.Add(new MenuListClass { text = ourname, image = f });
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
                HeightRequest = App.ScreenSize.Height,
                WidthRequest = App.ScreenSize.Width * .3,
                Content = innerStack
            };

            var masterStack = new StackLayout
            {
                BackgroundColor = Color.White,
                Orientation = StackOrientation.Vertical,
                MinimumWidthRequest = App.ScreenSize.Width * .35,
                WidthRequest = App.ScreenSize.Width * .35,
                HeightRequest = App.ScreenSize.Height /*- 52 - 36*/,
                //TranslationY = 8,
                Spacing = 0,
                Padding = new Thickness(0),
                StyleId = "menu",
                Children = { stackScroll }
            };

            Content = masterStack;
        }

        public void UpdateMenu(List<string> filenames)
        {
            menuList = new List<MenuListClass>();

            foreach (var f in filenames)
            {
                var file = f.Replace("_", " ").Split(' ');
                var ourname = string.Empty;
                if (file.Length != 1)
                {
                    ourname = string.Format("{0} {1}", UppercaseFirst(file[0]), file[1]);
                }
                else
                    ourname = UppercaseFirst(file[0]);

                menuList.Add(new MenuListClass { text = ourname, image = f });
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
                HeightRequest = App.ScreenSize.Height,
                WidthRequest = App.ScreenSize.Width * .3,
                Content = innerStack
            };

            var masterStack = new StackLayout
            {
                BackgroundColor = Color.White,
                Orientation = StackOrientation.Vertical,
                MinimumWidthRequest = App.ScreenSize.Width * .35,
                WidthRequest = App.ScreenSize.Width * .35,
                HeightRequest = App.ScreenSize.Height /*- 52 - 36*/,
                Spacing = 0,
                Padding = new Thickness(0),
                //TranslationY = 8,
                StyleId = "menu",
                Children = { stackScroll }
            };

            Content = masterStack;
        }

        StackLayout MenuListView(int i)
        {
            var imgIcon = new Image
            {
                WidthRequest = 42,
                HeightRequest = 42,
                Source = menuList[i].image
            };

            var lblText = new Label
            {
                FontSize = 18,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Blue,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = menuList[i].text
            };

            var tap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command((t) =>
                   {
                       MessagingCenter.Send(this, "Menu", "Close");
                       Page page = null;

                   }
                )
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
                        Orientation = StackOrientation.Vertical,
                        Children =
                        { imgIcon,
                            new StackLayout
                            {
                                Padding = new Thickness(8, 0, 0, 0),
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                                Children = { lblText }
                            }
                        }
                    }, new BoxView
                    {
                        WidthRequest = App.ScreenSize.Width,
                        HeightRequest = 1,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Color = Color.Gray
                    }
                },
            };
            stack.GestureRecognizers.Add(tap);

            return stack;
        }


    }
}


