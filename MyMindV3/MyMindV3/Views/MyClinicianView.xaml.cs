using Xamarin.Forms;
using Plugin.Media;
using MyMindV3.Languages;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using MvvmFramework.ViewModel;
using MvvmFramework;
#if DEBUG
using System.Diagnostics;
#endif

namespace MyMindV3.Views
{
    public partial class MyClinicianView : ContentPage
    {
        MyClinicianViewModel ViewModel => App.Locator.MyClinician;
        MediaFile file;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "Clinician")
                {
                    var res = ViewModel.Clinician;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ViewModel.ClinicianUser.FunFact = res.FunFact;
                        ViewModel.ClinicianUser.Name = res.Name;
                        ViewModel.ClinicianUser.Role = res.WhatIDo;
                        ViewModel.ClinicianUser.Phone = res.ContactNumber;
                    });
                }
            };
        }

        public MyClinicianView()
        {
            InitializeComponent();
            BindingContext = ViewModel.ClinicianUser;
            GetImage();
            GetDetails();
            if (ViewModel.SystemUser.IsAuthenticated == 3)
            {
                CreateImageClick();
            }
            else
                btnUpdate.IsVisible = false;
            ViewModel.ClinicianUser.IsClinician = ViewModel.SystemUser.IsAuthenticated == 3;

            vwRefresh.IsPullToRefreshEnabled = true;
            vwRefresh.RefreshColor = Color.Blue;
            vwRefresh.RefreshCommand = new Command(() =>
            {
                GetDetails();
                Device.BeginInvokeOnMainThread(() => { GetImage(); vwRefresh.IsRefreshing = false; });
            });
        }

        void GetImage()
        {
            var f = ViewModel.GetClinicianImage;
            if (ViewModel.SystemUser.IsAuthenticated == 3)
                imgClinician.Source = "male_female.png";
            else
            {
                ViewModel.ImageFilename = ViewModel.Filename = string.Format("{0}.jpg", ViewModel.GetClinicianImage);
                ViewModel.IsUser = ViewModel.SystemUser.IsAuthenticated == 2 ? true : false;
                if (ViewModel.FileExists)
                    imgClinician.Source = ImageSource.FromFile(string.Format("{0}/{1}", ViewModel.GetCurrentFolder, string.Format("{0}.jpg", ViewModel.GetClinicianImage)));
                else
                    imgClinician.Source = !string.IsNullOrEmpty(f) ? ImageSource.FromStream(() => ViewModel.GetProfileImage) : "male_female.png";
            }
        }

        void GetDetails()
        {
            ViewModel.GetClinicianDetails();
        }

        void Update_Profile(object s, EventArgs e)
        {
            ViewModel.UpdateProfile(ClinRoleInput.Text, ClinFunFactInput.Text, ClinPhoneInput.Text);
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
                                        ViewModel.UpdateSystemUser();
                                        await Send.UploadPicture(file.Path, ViewModel.ClinicianUser.Guid, ViewModel.SystemUser.APIToken);
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
