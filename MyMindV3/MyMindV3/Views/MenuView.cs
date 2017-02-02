using Xamarin.Forms;
using System.Collections.Generic;
using MyMindV3.Languages;
using Messier16.Forms.Controls;
using System;

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

        bool SetAll { get; set; }
        bool SetLocal { get; set; }
        bool SetNational { get; set; }
        bool SetWeb { get; set; }

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
            menuList = new List<MenuListClass>();
            foreach (var f in filenames)
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

            var chkAll = new Checkbox();
            var chkNational = new Checkbox();
            var chkLocal = new Checkbox();
            var chkWebOnly = new Checkbox();
            chkAll.SetBinding(Checkbox.CheckedProperty, new Binding(SetAll.ToString()));
            chkNational.SetBinding(Checkbox.CheckedProperty, new Binding(SetNational.ToString()));
            chkLocal.SetBinding(Checkbox.CheckedProperty, new Binding(SetLocal.ToString()));
            chkWebOnly.SetBinding(Checkbox.CheckedProperty, new Binding(SetWeb.ToString()));
            var chkAllTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() => MessagingCenter.Send(this, "ChkAll", chkAll.Checked))
            };
            var chkNationalTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() => MessagingCenter.Send(this, "chkNational", chkAll.Checked))
            };
            var chkLocalTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() => MessagingCenter.Send(this, "chkLocal", chkAll.Checked))
            };
            var chkWebTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() => MessagingCenter.Send(this, "chkWebOnly", chkAll.Checked))
            };
            chkAll.GestureRecognizers.Add(chkAllTap);
            chkNational.GestureRecognizers.Add(chkNationalTap);
            chkLocal.GestureRecognizers.Add(chkLocalTap);
            chkWebOnly.GestureRecognizers.Add(chkWebTap);

            var topStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    lblResource,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            chkAll,
                            new Label
                            {
                                Text = Langs.Menu_All,
                                FontSize = 12,
                                TextColor = Color.Blue,
                                VerticalTextAlignment = TextAlignment.Center
                            }
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                            TranslationY = -16,
                        Children =
                        {
                            chkNational,
                            new Label
                            {
                                Text = Langs.Menu_National,
                                FontSize = 12,
                                TextColor = Color.Blue,
                                VerticalTextAlignment = TextAlignment.Center
                            }
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        TranslationY = -32,
                        Children =
                        {
                            chkLocal,
                            new Label
                            {
                                Text = Langs.Menu_Local,
                                FontSize = 12,
                                TextColor = Color.Blue,
                                VerticalTextAlignment = TextAlignment.Center
                            }
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        TranslationY = -48,
                        Children =
                        {
                            chkWebOnly,
                            new Label
                            {
                                Text = Langs.Menu_Web,
                                FontSize = 12,
                                TextColor = Color.Blue,
                                VerticalTextAlignment = TextAlignment.Center
                            }
                        }
                    },
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

        public MenuView(List<string> filenames, bool web, bool all, bool nat, bool local)
        {
            SetAll = all;
            SetLocal = local;
            SetNational = nat;
            SetWeb = web;
            Content = GenerateUI(filenames);
        }

        public void UpdateMenu(List<string> filenames, bool web, bool all, bool nat, bool local)
        {
            SetAll = all;
            SetLocal = local;
            SetNational = nat;
            SetWeb = web;
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


