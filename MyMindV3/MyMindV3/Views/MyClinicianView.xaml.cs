using Xamarin.Forms;
using Plugin.Media;
using MyMindV3.Languages;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmFramework.ViewModel;
using MvvmFramework;
using MvvmFramework.Models;
#if DEBUG
using System.Diagnostics;
#endif

namespace MyMindV3.Views
{
    public partial class MyClinicianView : ContentPage
    {
        MyClinicianViewModel ViewModel => App.Locator.MyClinician;
        MediaFile file;

        public MyClinicianView()
        {
            InitializeComponent();
            BindingContext = ViewModel.ClinicianUser;
            GetImage();
            GetDetails().ConfigureAwait(true);
            if (ViewModel.SystemUser.IsAuthenticated == 3)
            {
                CreateImageClick();
            }
            else
                btnUpdate.IsVisible = false;
            ViewModel.ClinicianUser.IsClinician = ViewModel.SystemUser.IsAuthenticated == 3;

            vwRefresh.IsPullToRefreshEnabled = true;
            vwRefresh.RefreshColor = Color.Blue;
            vwRefresh.RefreshCommand = new Command(async () =>
            {
                //vwRefresh.IsRefreshing = true;
                await GetDetails().ContinueWith(
                (t) =>
                {
                    if (t.IsCompleted)
                    {
                        Device.BeginInvokeOnMainThread(() => { GetImage(); vwRefresh.IsRefreshing = false; });
                    }
                });
            });
        }

        void GetImage()
        {
            var f = ViewModel.ClinicianUser.UserImage;

            imgClinician.Source = !string.IsNullOrEmpty(f) ? ImageSource.FromStream(()=>ViewModel.GetClinicianImage(f)): "male_female.png";
        }

        async Task GetDetails()
        {
            await Send.SendData<List<ClinicianProfile>>("api/MyMind/GetClinicianProfile", "ClinicianGUID", 
                ViewModel.ClinicianUser.ClinicianGUID, "AuthToken", ViewModel.SystemUser.APIToken).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    var res = t.Result[0];
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ViewModel.ClinicianUser.FunFact = res.FunFact;
                        ViewModel.ClinicianUser.Name = res.Name;
                        ViewModel.ClinicianUser.Role = res.WhatIDo;
                        ViewModel.ClinicianUser.Phone = res.ContactNumber;
                    });
                }
            });
        }

        void Update_Profile(object s, EventArgs e)
        {
            var res = Send.SendData("api/MyMind/UpdateClinicianProfile", "ClinicianGUID", ViewModel.ClinicianUser.ClinicianGUID, 
                "AuthToken", ViewModel.ClinicianUser.APIToken,
                                    "WhatIDo", ClinRoleInput.Text, "FunFact", ClinFunFactInput.Text, "ContactNumber", ClinPhoneInput.Text).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    var userProfile = RegisterWeb(ViewModel.SystemUser);
                    var m = t.Result;
                }
            });
        }

        void CreateImageClick()
        {
            var imgTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(async (t) =>
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                    if (status == PermissionStatus.Granted)
                    {

                        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                        {
                            await DisplayAlert(Langs.MyProfile_ErrorCameraTitle, Langs.MyProfile_ErrorCameraMessage, Langs.Gen_OK);
                            return;
                        }

                        file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            Directory = "Directory",
                            Name = string.Format("{0}.jpg", ViewModel.SystemUser.Guid),
                            CompressionQuality = 92,
                            PhotoSize = PhotoSize.Small
                        });

                        if (file == null)
                            return;

                        App.Self.UserSettings.SaveSetting("ImageDirectory", file.Path.Substring(0, file.Path.LastIndexOf('/')), SettingType.String);


                        Device.BeginInvokeOnMainThread(async () =>
                            await DisplayAlert(Langs.Message_UploadingTitle, Langs.Message_UploadingMessage, Langs.Gen_OK).ContinueWith(async (w) =>
                                {
                                    if (w.IsCompleted)
                                    {
                                        var filesize = (int)file.GetStream().Length;
#if DEBUG
                                        Debug.WriteLine(file.Path);
#endif
                                        DependencyService.Get<IContent>().StoreFile(ViewModel.SystemUser.Guid, file.GetStream());
                                        ViewModel.SystemUser.UserImage = file.Path;
                                        ViewModel.UpdateSystemUser(V);
                                        //Send.HttpPost(file, _rootVM.SystemUser.Guid);
                                        await Send.UploadPicture(file.Path, ViewModel.SystemUser.Guid);
                                        file.Dispose();
                                    }
                                }));

                        imgClinician.Source = ImageSource.FromStream(() =>
                        {
                            var stream = file.GetStream();
                            return stream;
                        });
                    }
                    else
                    {
                        var res = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera).ContinueWith(async (arg) =>
                        {
                            if (arg.IsCompleted)
                            {
                                PermissionStatus state = PermissionStatus.Unknown;
                                if (arg.Result.TryGetValue(Permission.Camera, out state))
                                {
                                    if (state != PermissionStatus.Granted)
                                        await DisplayAlert(Langs.Camera_Permissions, Langs.Camera_Permissions_Message, Langs.Gen_OK);
                                }
                            }
                        });
                    }
                })
            };
            if (ViewModel.SystemUser.IsAuthenticated == 3)
                imgClinician.GestureRecognizers.Add(imgTap);
        }
    }
}
