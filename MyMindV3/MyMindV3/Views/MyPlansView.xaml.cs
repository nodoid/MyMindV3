using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System;
using MvvmFramework.Models;
using MvvmFramework.ViewModel;
using MvvmFramework;
using MyMindV3.Languages;
using System.Threading.Tasks;

namespace MyMindV3.Views
{
    public partial class MyPlansView : ContentPage
    {
        IEnumerable<ClientPlan> resources;
        MyPlansViewModel ViewModel => App.Locator.MyPlans;
        string guidToUse;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsBusy")
                {
                    ViewModel.SpinnerMessage = Langs.Gen_PleaseWait;
                    ViewModel.SpinnerTitle = Langs.Data_DownloadTitle;
                    Device.BeginInvokeOnMainThread(() => DependencyService.Get<INetworkSpinner>().NetworkSpinner(ViewModel.IsBusy, ViewModel.SpinnerTitle, ViewModel.SpinnerMessage));
                }
                if (e.PropertyName == "MyPlan")
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        resources = ViewModel.MyPlan;
                        PlanList.ItemsSource = resources;
                        PlanList.BackgroundColor = Color.FromHex("022330");
                        PlanList.ItemTemplate = new DataTemplate(typeof(MyPlansListCell));
                        PlanList.ItemSelected += Handle_Clicked;
                        PlanList.IsPullToRefreshEnabled = true;
                        PlanList.Refreshing += (object s, EventArgs ea) =>
                        {
                            PlanList.IsRefreshing = true;
                            ViewModel.GuidToUse = guidToUse;
                            resources = ViewModel.MyPlan;
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                PlanList.ItemsSource = null;
                                PlanList.ItemsSource = resources;
                                PlanList.IsRefreshing = false;
                            });
                        };
                    });
                }
            };
        }


        public MyPlansView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
            SetValues();
        }

        void SetValues()
        {
            guidToUse = string.Empty;
            if (ViewModel.SystemUser.IsAuthenticated != 3)
                guidToUse = ViewModel.SystemUser.Guid;
            else
                guidToUse = ViewModel.ClinicianUser.ClinicianGUID;

            ViewModel.GuidToUse = guidToUse;
            ViewModel.GetClientPlans();
        }


        void Handle_Clicked(object sender, SelectedItemChangedEventArgs e)
        {
            var id = e.SelectedItem as ClientPlan;
            var file = resources?.FirstOrDefault(t => t.FileID == Convert.ToInt32(id.FileID)).FileName;
            //file = file.Replace(" ", "");
            if (!string.IsNullOrEmpty(file))
            {
                var path = string.Format("{0}/{1}", ViewModel.GetCurrentFolder, file);
                //var path = string.Format("{0}/{1}", App.Self.ContentDirectory, file);
                if (!DependencyService.Get<IContent>().FileExists(path))
                {
                    ViewModel.IsBusy = true;
                    GetData.GetFile(id.FileID.ToString(), ViewModel.SystemUser.Guid, ViewModel.SystemUser.APIToken, file).ContinueWith((t) =>
                    {
                        if (t.IsCompleted)
                        {
                            ViewModel.IsBusy = false;
                            ViewModel.Filename = id.FileName;
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
                    ViewModel.Filename = id.FileName;
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
