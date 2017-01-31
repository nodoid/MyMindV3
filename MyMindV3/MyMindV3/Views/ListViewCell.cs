﻿using Xamarin.Forms;
using System.Diagnostics;
using MvvmFramework.Models;

namespace MyMindV3.Views
{
    public class ListViewCell : ViewCell
    {
        public ListViewCell()
        {
            var lblTitle = new Label
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                TextColor = Color.Blue,
            };
            lblTitle.SetBinding(Label.TextProperty, new Binding("Title"));

            var lblCategory = new Label
            {
                FontSize = 12,
                TextColor = Color.Green
            };
            lblCategory.SetBinding(Label.TextProperty, new Binding("Category"));

            var imgIcon = new Image
            {
                HeightRequest = 56,
                WidthRequest = 56
            };
            imgIcon.SetBinding(Image.SourceProperty, new Binding("ImageIcon"));

            var imgStar1 = new Image
            {
                WidthRequest = 16,
                HeightRequest = 16
            };
            imgStar1.SetBinding(Image.SourceProperty, new Binding("StarRatings[0]"));

            var imgStar2 = new Image
            {
                WidthRequest = 16,
                HeightRequest = 16
            };
            imgStar2.SetBinding(Image.SourceProperty, new Binding("StarRatings[1]"));

            var imgStar3 = new Image
            {
                WidthRequest = 16,
                HeightRequest = 16,
            };
            imgStar3.SetBinding(Image.SourceProperty, new Binding("StarRatings[2]"));

            var imgStar4 = new Image
            {
                WidthRequest = 16,
                HeightRequest = 16,
            };
            imgStar4.SetBinding(Image.SourceProperty, new Binding("StarRatings[3]"));

            var imgStar5 = new Image
            {
                WidthRequest = 16,
                HeightRequest = 16,
            };
            imgStar5.SetBinding(Image.SourceProperty, new Binding("StarRatings[4]"));

            var imgStar6 = new Image
            {
                WidthRequest = 16,
                HeightRequest = 16,
            };
            imgStar6.SetBinding(Image.SourceProperty, new Binding("StarRatings[5]"));

            var starStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(2, 0),
                Children = { imgStar1, imgStar2, imgStar3, imgStar4, imgStar5, imgStar6 }
            };
            starStack.SetBinding(StackLayout.ClassIdProperty, new Binding("Id"));

            var imgWord = new Image
            {
                Source = "w",
                WidthRequest = 20,
                HeightRequest = 20,
            };
            var imgPDF = new Image
            {
                Source = "p",
                WidthRequest = 20,
                HeightRequest = 20,
            };
            var imgHtml = new Image
            {
                Source = "h",
                WidthRequest = 20,
                HeightRequest = 20,
            };
            imgWord.SetBinding(Image.IsVisibleProperty, new Binding("HasW"));
            imgPDF.SetBinding(Image.IsVisibleProperty, new Binding("HasR"));
            imgHtml.SetBinding(Image.IsVisibleProperty, new Binding("HasH"));

            var lblDistance = new Label
            {
                TextColor = Color.Green,
                FontSize = 12
            };
            lblDistance.SetBinding(Label.TextProperty, new Binding("Distance"));

            var resourceStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
            };

            this.BindingContextChanged += (sender, e) =>
            {
                var ts = BindingContext as ListviewModel;
                if (ts != null)
                {
#if DEBUG
                    Debug.WriteLine("H={0}, R={1}, W={2}", ts.HasH ? 1 : 0, ts.HasR ? 1 : 0, ts.HasW ? 1 : 0);
#endif
                    if (ts.HasH && ts.HasR && ts.HasW)
                    {
                        resourceStack.Children.Add(
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                Children = { imgWord, imgPDF }
                            });
                        resourceStack.Children.Add(imgHtml);
                    }
                    else
                    {
                        resourceStack.Children.Add(
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            Children = { imgPDF, imgWord, imgHtml }
                        });
                    }
                }
            };

            var theStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(4),
                Children =
                {
                    new StackLayout
                    {
                        WidthRequest = App.ScreenSize.Width * .1,
                        MinimumWidthRequest = App.ScreenSize.Width * .1,
                        VerticalOptions = LayoutOptions.Start,
                        Children = {imgIcon}
                    },
                    new StackLayout
                    {
                        WidthRequest = App.ScreenSize.Width * .65,
                        MinimumWidthRequest = App.ScreenSize.Width * .60,
                        Orientation = StackOrientation.Vertical,
                        Children =
                        {
                            lblTitle,
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new Label{Text = "Rated",FontSize = 12},
                                    starStack
                                }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children = {new Label{Text="Category:", FontSize = 12, TextColor = Color.Green}, lblCategory}
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new Label {Text="Distance ", FontSize = 12, TextColor = Color.Green},
                                    lblDistance,
                                    new Label {Text=" miles", FontSize = 12, TextColor = Color.Green},
                                }
                            }
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        WidthRequest = App.ScreenSize.Width * .25,
                        MinimumWidthRequest = App.ScreenSize.Width *.25,
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Vertical,
                                Children =
                                {
                                    new Label {Text = "Resource format", FontSize = 13},
                                    resourceStack
                                }
                            }
                        }
                    }
                }
            };

            var tapRecogniser = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() =>
                {
                    App.Self.IdInUse = ((ListviewModel)BindingContext).Id;
                    var cv = new ModalView
                    {
                        IsClippedToBounds = true,
                        Rating = ((ListviewModel)BindingContext).CurrentRating,
                    };
                    Device.BeginInvokeOnMainThread(() => theStack.Children.Add(cv));
                })
            };
            theStack.GestureRecognizers.Add(tapRecogniser);

            View = theStack;
        }
    }
}