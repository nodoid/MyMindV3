using Xamarin.Forms;
using System.Collections.Generic;
using MvvmFramework.Models;
using MvvmFramework.ViewModel;
using MyMindV3.Languages;
using System.Linq;

namespace MyMindV3.Views
{
    public class MyResourcesView : ContentPage
    {
        SearchBar postcodeSearch;
        List<ListviewModel> dataList;
        List<Postcodes> postcodes;
        ListView listView;
        MenuView menu;

        MyResourcesViewModel ViewModel => App.Locator.MyResources;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Self.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsBusy")
                {
                    Device.BeginInvokeOnMainThread(() => DependencyService.Get<INetworkSpinner>().NetworkSpinner(ViewModel.IsBusy, ViewModel.SpinnerTitle, ViewModel.SpinnerMessage));
                }
                if (e.PropertyName == "NewIconRating")
                {
                    var bc = dataList[App.Self.IdInUse];
                    bc.CurrentRating = App.Self.NewIconRating;
                    bc.StarRatings = ViewModel.ConvertRatingToStars(bc.CurrentRating);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        listView.ItemsSource = null;
                        listView.ItemsSource = dataList;
                    });
                }
            };
        }

        public MyResourcesView()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            if (Device.OS == TargetPlatform.iOS)
                Padding = new Thickness(0, 20, 0, 0);
            BackgroundColor = Color.FromHex("022330");
            CreateUI();
        }

        void CreateUI()
        {
            this.BackgroundColor = Color.White;

            var imgGPS = new Image
            {
                Source = "gps",
                WidthRequest = 36,
                HeightRequest = 36
            };

            postcodeSearch = new SearchBar
            {
                WidthRequest = App.ScreenSize.Width * .75,
                Placeholder = Langs.MyResources_Postcode,
                BackgroundColor = Color.White,
                TextColor = Color.Blue,
                PlaceholderColor = Color.Gray,
                SearchCommand = new Command(() =>
                {
                    ViewModel.SearchPostcode = postcodeSearch.Text.Replace(" ", "").ToLowerInvariant();
                    ViewModel.GetAvailablePostcodes();
                    ViewModel.GetUIList();
                    dataList = ViewModel.UIList;
                    postcodes = ViewModel.AvailablePostcodes;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (listView.ItemsSource != null)
                            listView.ItemsSource = null;
                        listView.ItemsSource = dataList;
                        menu.UpdateMenu(ViewModel.GetResourceFilenames(dataList?.Select(t => t.Category).ToList()));
                        ViewModel.IsBusy = false;
                    });
                })
            };

            var searchStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(1),
                BackgroundColor = Color.FromHex("022330"),
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = App.ScreenSize.Width,
                HeightRequest = 42,
                Children =
                {
                    new BoxView{BackgroundColor = Color.FromHex("022330"), WidthRequest = App.ScreenSize.Width * .1},
                    postcodeSearch,
                    new StackLayout {WidthRequest = App.ScreenSize.Width *.15, HorizontalOptions = LayoutOptions.Start, Children = { imgGPS}}
                }
            };

            menu = new MenuView(ViewModel.GetResourceFilenames(dataList?.Select(t => t.Category).ToList()));
            menu.SizeChanged += (sender, e) => { menu.HeightRequest = App.ScreenSize.Height - searchStack.HeightRequest; };
            menu.BackgroundColor = Color.Purple;

            listView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(ListViewCell)),
                HasUnevenRows = true,
                HeightRequest = App.ScreenSize.Height - 32,
                BackgroundColor = Color.FromHex("022330"),
                SeparatorVisibility = SeparatorVisibility.None,
                IsPullToRefreshEnabled = true,
            };

            listView.RefreshCommand = new Command(() =>
                {
                    listView.IsRefreshing = true;
                    ViewModel.GetUIList();
                    dataList = ViewModel.UIList;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        listView.ItemsSource = null;
                        listView.ItemsSource = dataList;
                        listView.IsRefreshing = false;
                        menu.UpdateMenu(ViewModel.GetResourceFilenames(dataList?.Select(t => t.Category).ToList()));
                    });
                });

            // topbar



            var imgTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command((r) =>
                {
                    if (App.Self.Location != null)
                    {
                        ViewModel.SpinnerMessage = Langs.Gen_PleaseWait;
                        ViewModel.SpinnerTitle = Langs.Data_DownloadTitle;
                        ViewModel.IsBusy = true;
                        var myPostcode = ViewModel.GetMyPostcode;

                        if (!string.IsNullOrEmpty(myPostcode))
                        {
                            ViewModel.SearchPostcode = myPostcode;
                            ViewModel.GetAvailablePostcodes();
                            var postcodes = ViewModel.AvailablePostcodes;
                            ViewModel.GetUIList();
                            dataList = ViewModel.UIList;
                            Device.BeginInvokeOnMainThread(() =>
                             {
                                 if (listView.ItemsSource != null)
                                     listView.ItemsSource = null;
                                 listView.ItemsSource = dataList;
                                 ViewModel.IsBusy = false;
                             });
                        }
                        else
                        {
                            ViewModel.IsBusy = false;
                            DisplayAlert(Langs.Error_NetworkTitle, Langs.Error_PostcodeMessage, Langs.Gen_OK);
                        }
                    }
                    else
                    {
                        ViewModel.IsBusy = false;
                        DisplayAlert(Langs.Error_NetworkTitle, Langs.Error_GeolocationMessage, Langs.Gen_OK);
                    }
                })
            };
            imgGPS.GestureRecognizers.Add(imgTap);

            var imgButton = new Button
            {
                Image = "right",
                WidthRequest = 12,
                HeightRequest = 32
            };

            var innerStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    imgButton,
                    listView
                }
            };

            imgButton.Clicked += delegate
            {
                var bounds = innerStack.Children[1].Bounds;
                if (!App.Self.PanelShowing)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                        {
                            innerStack.WidthRequest = innerStack.Width + menu.Content.WidthRequest;

                            if (innerStack.Children.Count == 2)
                            {
                                innerStack.Children.Insert(0, menu);
                            }

                            var origBounds = innerStack.Children[0].Bounds;
                            await innerStack.Children[0].LayoutTo(origBounds, 250, Easing.CubicIn);
                            innerStack.Children[2].Opacity = .5;
                            App.Self.PanelShowing = true;
                        });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                {
                    if (innerStack.Children.Count > 2)
                    {
                        innerStack.Children[0].LayoutTo(bounds, 250, Easing.CubicOut).ContinueWith((t) =>
                    {
                        if (t.IsCompleted)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                                    {
                                        innerStack.Children[2].Opacity = 1;
                                        App.Self.PanelShowing = false;
                                        innerStack.Children.RemoveAt(0);
                                    });
                        }
                    });
                    }
                });
                }
            };

            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Children =
                {
                    searchStack,
                    innerStack
                }
            };
        }
    }
}

