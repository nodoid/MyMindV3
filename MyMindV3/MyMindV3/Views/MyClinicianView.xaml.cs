using MyMindV3.ViewModels;
using Xamarin.Forms;
using Plugin.Media;
using MyMindV3.Languages;
using Plugin.Media.Abstractions;
using MyMindV3.Data;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyMindV3.Models;
#if DEBUG
using System.Diagnostics;
#endif

namespace MyMindV3.Views
{
    public partial class MyClinicianView : ContentPage
    {
        private RootViewModel _rootVM;
        SystemUserDB _database;
        MediaFile file;

        public MyClinicianView(RootViewModel rootVM)
        {
            InitializeComponent();
            RootVM = rootVM;
            BindingContext = RootVM.ClinicianUser;
            GetImage();
            GetDetails().ConfigureAwait(true);
            if (RootVM.SystemUser.IsAuthenticated == 3)
            {
                CreateImageClick();
            }
            else
                btnUpdate.IsVisible = false;
            RootVM.ClinicianUser.IsClinician = RootVM.SystemUser.IsAuthenticated == 3;

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
            var f = _rootVM.ClinicianUser.UserImage;

            imgClinician.Source = !string.IsNullOrEmpty(f) ? f : "male_female.png";
        }

        async Task GetDetails()
        {
            await Send.SendData<List<ClinicianProfile>>("api/MyMind/GetClinicianProfile", "ClinicianGUID", RootVM.ClinicianUser.ClinicianGUID, "AuthToken", RootVM.SystemUser.APIToken).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    var res = t.Result[0];
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        RootVM.ClinicianUser.FunFact = res.FunFact;
                        RootVM.ClinicianUser.Name = res.Name;
                        RootVM.ClinicianUser.Role = res.WhatIDo;
                        RootVM.ClinicianUser.Phone = res.ContactNumber;
                    });
                }
            });
        }

        void Update_Profile(object s, EventArgs e)
        {
            var res = Send.SendData("api/MyMind/UpdateClinicianProfile", "ClinicianGUID", RootVM.ClinicianUser.ClinicianGUID, "AuthToken", RootVM.ClinicianUser.APIToken,
                                    "WhatIDo", ClinRoleInput.Text, "FunFact", ClinFunFactInput.Text, "ContactNumber", ClinPhoneInput.Text).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    var userProfile = _database.RegisterWeb(RootVM.SystemUser);
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
                            Name = string.Format("{0}.jpg", _rootVM.SystemUser.Guid),
                            CompressionQuality = 92,
                            PhotoSize = PhotoSize.Small
                        });

                        if (file == null)
                            return;

                        App.Self.UserSettings.SaveSetting("ImageDirectory", file.Path.Substring(0, file.Path.LastIndexOf('/')), SettingType.String);


                        Device.BeginInvokeOnMainThread(async () =>
                            await DisplayAlert("Uploading", "Your photo is currently being uploaded to our servers. This may take some time depending on your connection speed", "OK").ContinueWith(async (w) =>
                                {
                                    if (w.IsCompleted)
                                    {
                                        var filesize = (int)file.GetStream().Length;
#if DEBUG
                                        Debug.WriteLine(file.Path);
#endif
                                        DependencyService.Get<IContent>().StoreFile(_rootVM.SystemUser.Guid, file.GetStream());
                                        _database = new SystemUserDB();
                                        _rootVM.SystemUser.UserImage = file.Path;
                                        _database.UpdateSystemUser(_rootVM.SystemUser);
                                        //Send.HttpPost(file, _rootVM.SystemUser.Guid);
                                        await Send.UploadPicture(file.Path, _rootVM.SystemUser.Guid);
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
            if (RootVM.SystemUser.IsAuthenticated == 3)
                imgClinician.GestureRecognizers.Add(imgTap);
        }

        // provide access to RootVM
        public RootViewModel RootVM
        {
            get { return _rootVM; }
            set
            {
                if (value != _rootVM)
                {
                    _rootVM = value;
                    OnPropertyChanged("RootVM");
                }
            }
        }
    }
}
