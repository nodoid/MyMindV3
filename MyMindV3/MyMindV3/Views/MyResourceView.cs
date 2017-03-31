using Xamarin.Forms;
using MvvmFramework.Models;
using MvvmFramework.ViewModel;
using MyMindV3.Languages;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using MvvmFramework;
using MvvmFramework.Enums;
using System;
using System.Collections.ObjectModel;

namespace MyMindV3.Views
{
    public class MyResourcesView : ContentPage
    {
        SearchBar postcodeSearch;

        static ObservableCollection<ListviewModel> dataList { get; set; }
        ListView listView;
        MenuView menu;
        Image imgLeft, imgRight;
        Label lblNext, lblPrevious;

        bool FirstRun { get; set; } = true;
        bool GeoEnabled { get; set; } = false;

        MyResourcesViewModel ViewModel => App.Locator.MyResources;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            App.Self.PropertyChanged += Self_PropertyChanged;

            MessagingCenter.Subscribe<MenuView, int>(this, "buttonClicked", (arg1, arg2) =>
            {
                if (arg2 != ViewModel.SearchSelected)
                {
                    if (listView.ItemsSource != null)
                    {
                        listView.ItemsSource = null;
                        ViewModel.SearchSelected = arg2;
                        switch (arg2)
                        {
                            case 0:
                                ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National, Sorting.Distance);
                                break;
                            case 1:
                                ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National, Sorting.Rating);
                                break;
                            case 2:
                                ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National, Sorting.AZ);
                                break;
                            case 3:
                                ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National, Sorting.MostPopular);
                                break;
                        }
                        Device.BeginInvokeOnMainThread(() => listView.ItemsSource = ViewModel.UIList);
                    }
                }
            });

            MessagingCenter.Subscribe<MenuView, int>(this, "catButtonClicked", (arg1, arg2) =>
             {
                 if (arg2 != ViewModel.SearchCategory)
                 {
                     if (listView.ItemsSource != null)
                     {

                         listView.ItemsSource = null;
                         ViewModel.SearchCategory = arg2;
                         switch (ViewModel.SearchSelected)
                         {
                             case 0:
                                 ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National, Sorting.Distance);
                                 break;
                             case 1:
                                 ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National, Sorting.Rating);
                                 break;
                             case 2:
                                 ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National, Sorting.AZ);
                                 break;
                             case 3:
                                 ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National, Sorting.MostPopular);
                                 break;
                         }
                         Device.BeginInvokeOnMainThread(() => listView.ItemsSource = ViewModel.UIList);
                     }
                 }
             });

            MessagingCenter.Subscribe<ListViewCell, string>(this, "brokenlink", (arg1, arg2) =>
            {
                var id = Convert.ToInt32(arg2);
                ViewModel.ReportLink(id);
            });

            MessagingCenter.Subscribe<ListViewCell, string>(this, "LaunchWeb", async (arg1, arg2) =>
            {
                if (!string.IsNullOrEmpty(arg2))
                {
                    var urlid = dataList.FirstOrDefault(t => t.Url == arg2).Id;
                    ViewModel.SendTrackingInformation(ViewModel.GetIsClinician ?
                        ActionCodes.Clinician_Resource_Accessed :
                        (ViewModel.SystemUser.IsAuthenticated == 1 ? ActionCodes.Member_Resource_Accessed :
                        ActionCodes.User_Resource_Accessed), urlid, DateTime.Now.ToString("g"));
                    await Navigation.PushAsync(new MyWebview(arg2));
                }
            });

            MessagingCenter.Subscribe<ListViewCell, string>(this, "LaunchWeb", async (arg1, arg2) =>
            {
                if (!string.IsNullOrEmpty(arg2))
                {
                    var urlid = dataList.FirstOrDefault(t => t.Url == arg2).Id;
                    ViewModel.SendTrackingInformation(ViewModel.GetIsClinician ?
                        ActionCodes.Clinician_Resource_Accessed :
                        (ViewModel.SystemUser.IsAuthenticated == 1 ? ActionCodes.Member_Resource_Accessed :
                        ActionCodes.User_Resource_Accessed), urlid, DateTime.Now.ToString("g"));
                    await Navigation.PushAsync(new MyWebview(arg2));
                }
            });

            MessagingCenter.Subscribe<ListViewCell, string>(this, "html", async (arg1, arg2) =>
            {
                if (!string.IsNullOrEmpty(arg2))
                {
                    var urlid = dataList.FirstOrDefault(t => t.Url == arg2).Id;
                    ViewModel.SendTrackingInformation(ViewModel.GetIsClinician ?
                        ActionCodes.Clinician_Resource_Accessed :
                        (ViewModel.SystemUser.IsAuthenticated == 1 ? ActionCodes.Member_Resource_Accessed :
                        ActionCodes.User_Resource_Accessed), urlid, DateTime.Now.ToString("g"));
                    await Navigation.PushAsync(new MyWebview(arg2));
                }
            });

            MessagingCenter.Subscribe<ListViewCell, string>(this, "word", (arg1, arg2) =>
            {
                if (!string.IsNullOrEmpty(arg2))
                {
                    var urlid = dataList.FirstOrDefault(t => t.Id.ToString() == arg2);
                    GrabAndDisplayFile(urlid);
                }
            });

            MessagingCenter.Subscribe<ListViewCell, string>(this, "pdf", (arg1, arg2) =>
            {
                if (!string.IsNullOrEmpty(arg2))
                {
                    var urlid = dataList.FirstOrDefault(t => t.Id.ToString() == arg2);
                    GrabAndDisplayFile(urlid);
                }
            });
        }

        void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
            {
                ViewModel.SpinnerTitle = FirstRun ? Langs.Data_DownloadTitle : Langs.MyResources_Sorting;
                Device.BeginInvokeOnMainThread(() => DependencyService.Get<INetworkSpinner>().NetworkSpinner(ViewModel.IsBusy,
                                                                                                             ViewModel.SpinnerTitle,
                                                                                                             ViewModel.SpinnerMessage));
            }
            if (e.PropertyName == "DisableNationalBackPageButton")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    imgLeft.Source = !ViewModel.DisableNationalBackPageButton ? "left" : "leftgrey";
                    lblPrevious.TextColor = ViewModel.DisableNationalBackPageButton ? Color.Gray : Color.Blue;
                });
            }
            if (e.PropertyName == "DisableNationalNextPageButton")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    imgRight.Source = !ViewModel.DisableNationalNextPageButton ? "right" : "rightgrey";
                    lblNext.TextColor = ViewModel.DisableNationalNextPageButton ? Color.Gray : Color.Blue;
                });
            }
            if (e.PropertyName == "DisableLocalBackPageButton")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    imgLeft.Source = !ViewModel.DisableLocalBackPageButton ? "left" : "leftgrey";
                    lblPrevious.TextColor = ViewModel.DisableLocalNextPageButton ? Color.Gray : Color.Blue;
                });
            }
            if (e.PropertyName == "DisableLocalNextPageButton")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    imgRight.Source = !ViewModel.DisableLocalNextPageButton ? "right" : "rightgrey";
                    lblNext.TextColor = ViewModel.DisableLocalNextPageButton ? Color.Gray : Color.Blue;
                });
            }

            if (e.PropertyName == "UIList")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    listView.ItemsSource = null;
                    listView.ItemsSource = ViewModel.UIList;
                });
            }
        }

        void Self_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NewIconRating")
            {
                listView.BeginRefresh();
                if (dataList.Count == 0)
                {
                    ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National);
                    dataList = ViewModel.UIList;
                }
                var bc = dataList.FirstOrDefault(t => t.Id == App.Self.IdInUse);
                bc.CurrentRating = App.Self.NewIconRating;
                bc.StarRatings = ViewModel.ConvertRatingToStars(bc.CurrentRating, bc.CurrentRating);
                listView.ItemsSource = null;
                ViewModel.UpdateUIList(bc);
                dataList = ViewModel.UIList;
                Device.BeginInvokeOnMainThread(() =>
                                {
                                    listView.ItemsSource = dataList;
                                });
                ViewModel.SetRating(ViewModel.GetIsClinician);
                listView.EndRefresh();
            }

            if (e.PropertyName == "Location")
            {
                ViewModel.Speed = App.Self.Location.Speed;

                if (ViewModel.PositionChanged(App.Self.Location.Longitude, App.Self.Location.Latitude))
                {
                    ViewModel.IsBusy = true;
                    ViewModel.GetLocalResources(ViewModel.GetIsClinician);
                    if (ViewModel.ShowingLocal)
                    {
                        ViewModel.GetUIList(UIType.Local);
                        dataList = ViewModel.UIList;
                        if (dataList.Count != 0)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                         {
                             if (listView.ItemsSource != null)
                                 listView.ItemsSource = null;
                             listView.ItemsSource = dataList;
                             ViewModel.IsBusy = false;
                             SwapView(1);
                             menu.UpdateMenu(ViewModel.SearchSelected, ViewModel.SearchCategory, ViewModel.GetCategtoriesFromResource);
                         });
                        }
                    }
                    else
                        ViewModel.IsBusy = false;
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<MenuView>(this, "buttonChecked");
            MessagingCenter.Unsubscribe<MenuView>(this, "catButtonClicked");
            MessagingCenter.Unsubscribe<ListViewCell>(this, "LaunchWeb");
            MessagingCenter.Unsubscribe<ListViewCell>(this, "brokenlink");
            MessagingCenter.Unsubscribe<ListViewCell>(this, "html");
            MessagingCenter.Unsubscribe<ListViewCell>(this, "word");
            MessagingCenter.Unsubscribe<ListViewCell>(this, "pdf");
            App.Self.PropertyChanged -= Self_PropertyChanged;
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        public MyResourcesView()
        {
            BackgroundColor = Color.FromHex("022330");
            Title = Langs.MyResources_Title;
            ViewModel.CurrentLocalPage = ViewModel.CurrentNationalPage = 1;
            ViewModel.SearchSelected = 2;
            ViewModel.SearchCategory = 0;
            ViewModel.DisableNationalBackPageButton = ViewModel.DisableLocalBackPageButton = true;
            ViewModel.ShowingLocal = false;
            ViewModel.SearchPostcode = ViewModel.GetIsClinician ? "IG38XJ" : !string.IsNullOrEmpty(ViewModel.SystemUser.PostCode) ? ViewModel.SystemUser.PostCode : "IG38XJ";
            CreateUI();
        }

        void CreateUI()
        {
            listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                BindingContext = ViewModel.UIList,
                ItemTemplate = new DataTemplate(typeof(ListViewCell)),
                HasUnevenRows = true,
                BackgroundColor = Color.FromHex("022330"),
                SeparatorVisibility = SeparatorVisibility.None,
                IsPullToRefreshEnabled = true,
                HeightRequest = App.ScreenSize.Height * .6
            };

            ViewModel.SpinnerMessage = Langs.Gen_PleaseWait;
            Task.Run(() =>
            {
                ViewModel.IsBusy = true;
                ViewModel.GetLocalResources(ViewModel.GetIsClinician);
                ViewModel.GetNationalResources(ViewModel.GetIsClinician);
                ViewModel.GetUIList(UIType.National);
                dataList = ViewModel.UIList;
                Device.BeginInvokeOnMainThread(() => listView.ItemsSource = dataList);
                if (menu == null)
                {
                    menu = new MenuView(ViewModel.SearchSelected, ViewModel.SearchCategory, ViewModel.GetCategtoriesFromResource);
                }
                ViewModel.IsBusy = false;
            });

            imgLeft = new Image
            {
                HeightRequest = 32,
                WidthRequest = 32
            };

            imgRight = new Image
            {
                HeightRequest = 32,
                WidthRequest = 32,
                HorizontalOptions = LayoutOptions.End
            };

            if (ViewModel.ShowingLocal)
            {
                imgLeft.Source = ViewModel.DisableLocalBackPageButton ? "leftgrey" : "left";
                imgRight.Source = ViewModel.DisableLocalNextPageButton ? "rightgrey" : "right";
            }
            else
            {
                imgLeft.Source = ViewModel.DisableNationalBackPageButton ? "leftgrey" : "left";
                imgRight.Source = ViewModel.DisableNationalNextPageButton ? "rightgrey" : "right";
            }

            var leftTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() =>
                {
                    if ((ViewModel.ShowingLocal && !ViewModel.DisableLocalBackPageButton) || (!ViewModel.ShowingLocal && !ViewModel.DisableNationalBackPageButton))
                    {
                        ViewModel.BtnBackCommand.Execute(null);
                        ViewModel.IsBusy = true;
                        //if (ViewModel.ShowingLocal) ViewModel.GetLocalResources(ViewModel.GetIsClinician); else ViewModel.GetNationalResources(ViewModel.GetIsClinician);
                        ViewModel.GetUIList();
                        dataList = ViewModel.UIList;

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (listView.ItemsSource != null)
                                listView.ItemsSource = null;
                            listView.ItemsSource = dataList;
                            menu.UpdateMenu(ViewModel.SearchSelected, ViewModel.SearchCategory, ViewModel.GetCategtoriesFromResource);
                            ViewModel.IsBusy = false;
                        });
                    }
                })
            };

            var rightTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() =>
                {
                    if ((ViewModel.ShowingLocal && !ViewModel.DisableLocalNextPageButton) || (!ViewModel.ShowingLocal && !ViewModel.DisableNationalNextPageButton))
                    {
                        ViewModel.BtnForwardCommand.Execute(null);
                        ViewModel.IsBusy = true;
                        /*if (ViewModel.ShowingLocal)
                            ViewModel.GetLocalResources(ViewModel.GetIsClinician);
                        else
                            ViewModel.GetNationalResources(ViewModel.GetIsClinician);*/
                        ViewModel.GetUIList();
                        dataList = ViewModel.UIList;

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (listView.ItemsSource != null)
                                listView.ItemsSource = null;
                            listView.ItemsSource = dataList;
                            menu.UpdateMenu(ViewModel.SearchSelected, ViewModel.SearchCategory, ViewModel.GetCategtoriesFromResource);
                            ViewModel.IsBusy = false;
                        });
                    }
                })
            };

            imgLeft.GestureRecognizers.Add(leftTap);
            imgRight.GestureRecognizers.Add(rightTap);

            var imgGPS = new Image
            {
                Source = "gps",
                HeightRequest = 28,
                WidthRequest = 28,
                HorizontalOptions = LayoutOptions.Center
            };
            lblPrevious = new Label
            {
                Text = Langs.MyResources_Previous,
                //TextColor = ViewModel.DisableBackPageButton ? Color.Gray : Color.Blue,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 14,
            };
            lblNext = new Label
            {
                Text = Langs.MyResources_Next,
                //TextColor = ViewModel.DisableNextPageButton ? Color.Gray : Color.Blue,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.End,
                FontSize = 14,
            };

            if (ViewModel.ShowingLocal)
            {
                lblPrevious.TextColor = ViewModel.DisableLocalBackPageButton ? Color.Gray : Color.Blue;
                lblNext.TextColor = ViewModel.DisableLocalNextPageButton ? Color.Gray : Color.Blue;
            }
            else
            {
                lblNext.TextColor = ViewModel.DisableNationalNextPageButton ? Color.Gray : Color.Blue;
                lblPrevious.TextColor = ViewModel.DisableNationalBackPageButton ? Color.Gray : Color.Blue;
            }

            postcodeSearch = new SearchBar
            {
                WidthRequest = App.ScreenSize.Width * .75,
                Placeholder = Langs.MyResources_Postcode,
                BackgroundColor = Color.FromHex("022330"),
                TextColor = Device.OS == TargetPlatform.Android ? Color.White : Color.Blue,
                PlaceholderColor = Color.Gray,
                SearchCommand = new Command(() =>
                {
                    ViewModel.Speed = 0;
                    ViewModel.IsBusy = true;
                    ViewModel.SearchPostcode = postcodeSearch.Text.Replace(" ", "").ToUpperInvariant();
                    Task.Run(() =>
                    {

                        ViewModel.GetLocalResources(ViewModel.GetIsClinician);
                        if (ViewModel.ShowingLocal)
                        {
                            ViewModel.GetUIList();
                            dataList = ViewModel.UIList;
                            if (dataList.Count != 0)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    if (listView.ItemsSource != null)
                                        listView.ItemsSource = null;
                                    listView.ItemsSource = dataList;
                                    SwapView(1);
                                    menu.UpdateMenu(ViewModel.SearchSelected, ViewModel.SearchCategory, ViewModel.GetCategtoriesFromResource);
                                    ViewModel.IsBusy = false;
                                });
                            }
                        }
                        else
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
                //HeightRequest = 42,
                Children =
                {
                    new BoxView{BackgroundColor = Color.FromHex("022330"), WidthRequest = App.ScreenSize.Width * .12},
                    postcodeSearch,
                    new StackLayout {WidthRequest = App.ScreenSize.Width *.13, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, TranslationX = -8,Children = { imgGPS}}
                }
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
                        menu?.UpdateMenu(ViewModel.SearchSelected, ViewModel.SearchCategory, ViewModel.GetCategtoriesFromResource);
                    });
                });

            var imgTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command((r) =>
                {
                    if (App.Self.Location != null)
                    {
                        ViewModel.IsBusy = true;
                        ViewModel.Longitude = App.Self.Location.Longitude;
                        ViewModel.Latitude = App.Self.Location.Latitude;
                        ViewModel.Speed = App.Self.Location.Speed;
                        Task.Run(() =>
                        {
                            var myPostcode = ViewModel.GetMyPostcode;

                            if (!string.IsNullOrEmpty(myPostcode))
                            {
                                GeoEnabled = true;
                                ViewModel.SearchPostcode = myPostcode;
                                if (ViewModel.ShowingLocal)
                                    ViewModel.GetLocalResources(ViewModel.GetIsClinician);
                                else
                                    ViewModel.GetNationalResources(ViewModel.GetIsClinician);
                                ViewModel.GetUIList(ViewModel.ShowingLocal ? UIType.Local : UIType.National);
                                dataList = ViewModel.UIList;

                                Device.BeginInvokeOnMainThread(() =>
                                 {
                                     if (listView.ItemsSource != null)
                                         listView.ItemsSource = null;
                                     listView.ItemsSource = dataList;
                                     ViewModel.IsBusy = false;
                                     menu.UpdateMenu(ViewModel.SearchSelected, ViewModel.SearchCategory, ViewModel.GetCategtoriesFromResource);
                                 });
                            }
                            else
                            {
                                ViewModel.IsBusy = false;
                                DisplayAlert(Langs.Error_NetworkTitle, Langs.Error_PostcodeMessage, Langs.Gen_OK);
                            }
                        });
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

            var moveStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                HeightRequest = 56,
                TranslationY = -116,
                Children =
                {
                    new StackLayout
                    {
                        WidthRequest = App.ScreenSize.Width *.3,
                        MinimumWidthRequest = App.ScreenSize.Width * .3,
                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.Center,
                        Children = {imgLeft, lblPrevious}
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        WidthRequest = App.ScreenSize.Width *.4,
                    },
                    new StackLayout
                    {
                        WidthRequest = App.ScreenSize.Width *.3,
                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.End,
                        Children = {lblNext, imgRight}
                    },
                }
            };

            var innerStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = App.ScreenSize.Height,
                BackgroundColor = Color.FromHex("022330"),
                MinimumHeightRequest = App.ScreenSize.Height,
                Children =
                {
                    imgButton,
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

            var masterGrid = new Grid
            {
                WidthRequest = App.ScreenSize.Width,
                HeightRequest = App.ScreenSize.Height,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition {Width = App.ScreenSize.Width * .3},
                    new ColumnDefinition {Width = App.ScreenSize.Width * .3},
                    new ColumnDefinition {Width = App.ScreenSize.Width * .4}
                },
                ColumnSpacing = 2,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition {Height = 22},
                    new RowDefinition {Height = GridLength.Star}
                }
            };

            var lblNational = new Label
            {
                Text = Langs.MyResources_National,
                TextColor = Color.White,
                BackgroundColor = Color.Red,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            var lblLocal = new Label
            {
                Text = Langs.MyResources_Local,
                TextColor = Color.Red,
                BackgroundColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center
            };

            //SwapView(0);

            var stack = new StackLayout
            {
                WidthRequest = App.ScreenSize.Width,
                Orientation = StackOrientation.Vertical,
                Children = { listView }
            };

            var lblNationalTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() =>
                {
                    if (!string.IsNullOrEmpty(postcodeSearch.Text) || GeoEnabled)
                    {
                        if (!ViewModel.ShowingLocal)
                            return;
                        else
                        {
                            lblNational.BackgroundColor = Color.Red;
                            lblLocal.BackgroundColor = Color.White;
                            lblNational.TextColor = Color.White;
                            lblLocal.TextColor = Color.Red;
                            ViewModel.ShowingLocal = !ViewModel.ShowingLocal;
                            ViewModel.IsBusy = true;
                            Task.Run(() =>
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    stack.Children.RemoveAt(0);
                                    SwapView(0);
                                    stack.Children.Add(listView);
                                    ViewModel.IsBusy = false;
                                });
                            });
                        }
                    }
                })
            };

            var lblLocalTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() =>
                {
                    if (!string.IsNullOrEmpty(postcodeSearch.Text) || GeoEnabled)
                    {
                        if (ViewModel.ShowingLocal)
                            return;
                        else
                        {
                            listView.ItemsSource = null;
                            ViewModel.IsBusy = true;
                            ViewModel.ShowingLocal = !ViewModel.ShowingLocal;
                            Task.Run(() =>
                        {
                            ViewModel.GetUIList(UIType.Local);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                stack.Children.RemoveAt(0);
                                SwapView(1);
                                stack.Children.Add(listView);
                                lblNational.BackgroundColor = Color.White;
                                lblLocal.BackgroundColor = Color.Red;
                                lblNational.TextColor = Color.Red;
                                lblLocal.TextColor = Color.White;
                                ViewModel.IsBusy = false;
                            });
                        });
                        }
                    }
                })
            };

            lblNational.GestureRecognizers.Add(lblNationalTap);
            lblLocal.GestureRecognizers.Add(lblLocalTap);

            masterGrid.Children.Add(lblNational, 0, 0);
            masterGrid.Children.Add(lblLocal, 1, 0);
            masterGrid.Children.Add(stack, 0, 1);
            Grid.SetColumnSpan(stack, 3);

            innerStack.Children.Add(new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = { masterGrid, moveStack }
            });

            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Spacing = 0,
                Padding = new Thickness(0),
                Children =
                {
                    searchStack,
                    innerStack
                }
            };

            if (FirstRun)
            {
                ViewModel.GetUIList(UIType.National);
                dataList = ViewModel.UIList;
                Device.BeginInvokeOnMainThread(() => listView.ItemsSource = dataList);
                FirstRun = false;
            }
        }

        void SwapView(int view)
        {
            ViewModel.ShowingLocal = view != 0;
            ViewModel.GetUIList(view == 0 ? UIType.National : UIType.Local);
            listView.ItemsSource = null;
            dataList = ViewModel.UIList;
            Device.BeginInvokeOnMainThread(() => listView.ItemsSource = dataList);
            menu.UpdateMenu(ViewModel.SearchSelected, ViewModel.SearchCategory, ViewModel.GetCategtoriesFromResource);
        }

        void GrabAndDisplayFile(ListviewModel view)
        {
            var file = view.Url;
            //file = file.Replace(" ", "");
            if (!string.IsNullOrEmpty(file))
            {
                var path = string.Format("{0}/{1}", ViewModel.GetCurrentFolder, file);
                //var path = string.Format("{0}/{1}", App.Self.ContentDirectory, file);
                if (!DependencyService.Get<IContent>().FileExists(path))
                {
                    ViewModel.IsBusy = true;
                    GetData.GetFile(file, ViewModel.SystemUser.Guid, ViewModel.SystemUser.APIToken, file).ContinueWith((t) =>
                    {
                        if (t.IsCompleted)
                        {
                            ViewModel.IsBusy = false;
                            ViewModel.Filename = file.Split('/').Last();
                            var filetype = file.Split('.').Last().ToLower();
                            if (filetype == "jpg" || filetype == "jpeg" || filetype == "png")
                                //Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(new MyFileView(path)));
                                Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new MyFileView(file)));
                            else
                            {
                                Device.OnPlatform(iOS: () => DependencyService.Get<IFile>().OpenFileExternally(path),
                                                  Android: () =>
                                                  {
                                                      var mimetype = string.Empty;
                                                      switch (filetype)
                                                      {
                                                          case "doc":
                                                              mimetype = "application/msword";
                                                              break;
                                                          case "docx":
                                                              mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                                                              break;
                                                          case "rtf":
                                                              mimetype = "application/rtf";
                                                              break;
                                                          case "pdf":
                                                              mimetype = "application/pdf";
                                                              break;
                                                      }
                                                      DependencyService.Get<IFileAndroid>().launchfile(path, mimetype);
                                                  });
                            }
                        }
                    });
                }
                else
                {
                    ViewModel.Filename = file.Split('/').Last();
                    var filetype = file.Split('.').Last().ToLower();
                    if (filetype == "jpg" || filetype == "jpeg" || filetype == "png")
                        //Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(new MyFileView(path)));
                        Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new MyFileView(file)));
                    else
                    {
                        Device.OnPlatform(iOS: () => DependencyService.Get<IFile>().OpenFileExternally(path),
                                          Android: () =>
                                          {
                                              var mimetype = string.Empty;
                                              switch (filetype)
                                              {
                                                  case "doc":
                                                      mimetype = "application/msword";
                                                      break;
                                                  case "docx":
                                                      mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                                                      break;
                                                  case "rtf":
                                                      mimetype = "application/rtf";
                                                      break;
                                                  case "pdf":
                                                      mimetype = "application/pdf";
                                                      break;
                                              }
                                              DependencyService.Get<IFileAndroid>().launchfile(path, mimetype);
                                          });
                    }
                }
            }
        }
    }
}

