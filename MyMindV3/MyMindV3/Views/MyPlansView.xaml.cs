using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System;
using MvvmFramework.Models;
using MvvmFramework.ViewModel;
using MvvmFramework;

namespace MyMindV3.Views
{
    public partial class MyPlansView : ContentPage
    {
        IEnumerable<ClientPlan> resources;
        MyPlansViewModel ViewModel => App.Locator.MyPlans;

        public MyPlansView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
            SetValues();
        }


        private async void SetValues()
        {
            var guidToUse = string.Empty;
            if (RootVM.SystemUser.IsAuthenticated != 3)
                guidToUse = RootVM.SystemUser.Guid;
            else
                guidToUse = RootVM.ClinicianUser.ClinicianGUID;

            resources = await GetClientPlans(guidToUse);
            PlanList.ItemsSource = resources;
            PlanList.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null) return;
            };
            PlanList.IsPullToRefreshEnabled = true;
            PlanList.Refreshing += async (object sender, EventArgs e) =>
            {
                PlanList.IsRefreshing = true;
                await GetClientPlans(guidToUse).ContinueWith((r) =>
                {
                    if (r.IsCompleted)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            PlanList.ItemsSource = null;
                            PlanList.ItemsSource = r.Result;
                        });

                        PlanList.IsRefreshing = false;
                    }
                });
            };
        }

        async Task<IEnumerable<ClientPlan>> GetClientPlans(string guid)
        {
            var url = string.Format("/api/MyMind/GetMyPlans/{0}", guid);
            resources = await GetData.GetListObject<ClientPlan>(url);
            return resources;
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            var fileid = ((Button)sender).ClassId;
            var file = resources?.FirstOrDefault(t => t.FileID == Convert.ToInt32(fileid)).FileName;
            //file = file.Replace(" ", "");
            if (!string.IsNullOrEmpty(file))
            {
                var path = string.Format("{0}/{1}", App.Self.ContentDirectory, file);
                if (!DependencyService.Get<IContent>().FileExists(path))
                {
                    GetData.GetFile(fileid, RootVM.SystemUser.Guid, RootVM.SystemUser.APIToken, file).ContinueWith((t) =>
                    {
                        if (t.IsCompleted)
                        {
                            var filetype = file.Split('.').Last().ToLower();
                            if (filetype == "jpg" || filetype == "jpeg" || filetype == "png")
                                Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(new MyFileView(path)));
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
                    var filetype = file.Split('.').Last().ToLower();
                    if (filetype == "jpg" || filetype == "jpeg" || filetype == "png")
                        Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(new MyFileView(path)));
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
