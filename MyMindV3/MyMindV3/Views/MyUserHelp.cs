using System;

using Xamarin.Forms;
using MyMindV3.Languages;
using MvvmFramework.ViewModel;

#if DEBUG
using System.Diagnostics;
#endif

namespace MyMindV3
{
    public class MyHelpView : ContentPage
    {
        MyHelpViewModel ViewModel => App.Locator.MyHelp;
        bool isClinician;

        public MyHelpView(string clinic = "")
        {
            isClinician = !string.IsNullOrEmpty(clinic);
            CreateUI();
            Title = Langs.MyMind_Help;
            BackgroundColor = Color.FromHex("022330");
        }

        void CreateUI()
        {

            var view1 = !isClinician ? CreateView("a_myprofile.png", Langs.MyProfile_Title, Langs.Slider_MyProfile, 1, .55) :
                CreateView("a_mypatient.png", Langs.MyPatient_Title, Langs.Slider_MyPatient, 1, .55);
            var view2 = CreateView("b_myclinician.png", Langs.MyClinician_Title, Langs.Slider_MyClinician, 1, .55);
            var view3 = CreateView("c_myjourney.png", Langs.MyJourney_Title, Langs.Slider_MyJourney, 1, .55);
            var view4 = CreateView("d_mychat.png", Langs.MyMind_Chat, Langs.Slider_MyChat, 1, .55);
            var view5 = CreateView("e_myplans.png", Langs.MyPlans_Title, Langs.Slider_MyPlans, 1, .55);
            var view6 = CreateView("f_myresources.png", Langs.MyResources_Title, Langs.Slider_MyResources, 1, .55);
            var view7 = CreateView("g_myresources.png", Langs.MyResources_Title, Langs.Slider_MyResourcesRate, 1, .55);

            var slider = new SliderView(view1, App.ScreenSize.Height * 0.8, App.ScreenSize.Width)
            {
                BackgroundColor = Color.FromHex("022330"),
                TransitionLength = 200,
                MinimumSwipeDistance = 50
            };

            slider.Children.Add(view2);
            slider.Children.Add(view3);
            slider.Children.Add(view4);
            slider.Children.Add(view5);
            slider.Children.Add(view6);
            slider.Children.Add(view7);

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    slider
                }
            };
        }

        ContentView CreateView(string imageName, string mainHeading, string text, double width = 1, double multiplier = .8, double height = .45)
        {
            var img = new Image();
            try
            {
                img.Source = imageName;
                img.HeightRequest = App.ScreenSize.Height * height;
                img.WidthRequest = App.ScreenSize.Width;
                img.HorizontalOptions = LayoutOptions.CenterAndExpand;
                img.Aspect = width == 1 ? Aspect.AspectFit : Aspect.Fill;
            }
            catch (OutOfMemoryException ex)
            {
#if DEBUG
                Debug.WriteLine("Out of memory - {0}::{1}", ex.Message, ex.InnerException);
#endif
            }

            return new ContentView
            {
                Content = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    VerticalOptions = LayoutOptions.Start,
                    Children =
                    {
                        new StackLayout
                        {
                            Padding = new Thickness(0),
                            Children =
                            {
                                img
                            }
                        },
                        new StackLayout
                        {
                            VerticalOptions = LayoutOptions.Start,
                            Padding = new Thickness(0),
                            Children =
                            {
                                new StackLayout
                                {
                                    Children =
                                    {
                                        new Label
                                        {
                                            Text = mainHeading,
                                            TextColor = Color.White,
                                            FontSize = 22,
                                            LineBreakMode = LineBreakMode.WordWrap,
                                            WidthRequest = App.ScreenSize.Width * (mainHeading == Langs.MyMind_Help ? (Device.OS == TargetPlatform.iOS ? .8 : .9) : width / 1.75),
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            HorizontalOptions = LayoutOptions.Center
                                        },
                                        new Label
                                        {
                                            Text = text,
                                            TextColor = Color.White,
                                            FontSize = 14,
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            WidthRequest = App.ScreenSize.Width * .8,
                                            LineBreakMode = LineBreakMode.WordWrap,
                                            HorizontalOptions = LayoutOptions.Center
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            };
        }
    }
}

