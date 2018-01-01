using Xamarin.Forms;
using Plugin.Media;
using MyMindV3.Languages;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using MvvmFramework.ViewModel;
using MvvmFramework;
using System.Linq;
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
                if (e.PropertyName == "Filename")
                {
                    Device.BeginInvokeOnMainThread(() => imgClinician.Source = ImageSource.FromFile(string.Format("{0}/{1}", ViewModel.GetCurrentFolder, string.Format("{0}.jpg", ViewModel.Filename))));
                }
            };
        }

        public MyClinicianView()
        {
            InitializeComponent();
            DisplayProfileDetails.IsVisible = true;
            EditProfileDetails.IsVisible = false;
            BindingContext = ViewModel;
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

            ViewModel.ImageFilename = ViewModel.Filename = string.Format("{0}/{1}.jpg", ViewModel.GetCurrentFolder, ViewModel.GetClinicianImage);
            ViewModel.IsUser = ViewModel.SystemUser.IsAuthenticated != 2 ? true : false;
            if (ViewModel.FileExists && !ViewModel.LoadNewProfileImage)
                Device.BeginInvokeOnMainThread(() => imgClinician.Source = ImageSource.FromFile(ViewModel.ImageFilename));
            else
                Device.BeginInvokeOnMainThread(() => imgClinician.Source = !string.IsNullOrEmpty(ViewModel.ImageFilename) ? ImageSource.FromStream(() => ViewModel.GetProfileImage) : "male_female.png");
        }

        void GetDetails()
        {
            ViewModel.GetClinicianDetails();
        }

        void Update_Profile()
        {
            ViewModel.UpdateProfile(EditClinRoleInput.Text, EditClinFunFactInput.Text, EditClinPhoneInput.Text);
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
                            Name = string.Format("{0}.jpg", ViewModel.ClinicianUser.ClinicianGUID),
                            CompressionQuality = 92,
                            PhotoSize = PhotoSize.Small
                        });

                        if (file == null)
                            return;

                        //App.Self.UserSettings.SaveSetting("ImageDirectory", file.Path.Substring(0, file.Path.LastIndexOf('/')), SettingType.String);

                        var filename = string.Format("{0}/{1}", ViewModel.GetCurrentFolder, file.Path.Split('/').ToList().Last());
                        if (filename.Contains("_"))
                        {
                            var fn = filename.Split('_');
                            filename = fn[0] + ".jpg";
                        }

                        Device.BeginInvokeOnMainThread(async () =>
                            await DisplayAlert(Langs.Message_UploadingTitle, Langs.Message_UploadingMessage, Langs.Gen_OK).ContinueWith(async (w) =>
                                {
                                    if (w.IsCompleted)
                                    {
                                        var filesize = (int)file.GetStream().Length;
#if DEBUG
                                        Debug.WriteLine(file.Path);
#endif
                                        //DependencyService.Get<IContent>().StoreFile(ViewModel.SystemUser.Guid, file.GetStream());
                                        ViewModel.SystemUser.UserImage = filename;
                                        ViewModel.UpdateSystemUser();
                                        if (App.Self.IsConnected)
                                            await Send.UploadPicture(file.GetStream(), file.Path, ViewModel.ClinicianUser.ClinicianGUID, ViewModel.SystemUser.APIToken, ViewModel.SystemUser.IsAuthenticated.ToString()).ContinueWith((z) =>
                                        {
                                            if (z.IsCompleted && (!z.IsFaulted || !z.IsCanceled))
                                            {
                                                if (z.Result != "-1")
                                                {
                                                    ViewModel.SetProfileDateTime(z.Result);
                                                }
                                            }
                                        });
                                        else
                                            await DisplayAlert(Langs.Network_ErrorTitle, Langs.Network_ErrorMessage, Langs.Gen_OK);
                                        file.Dispose();
                                    }
                                }));

                        //imgClinician.Source = ImageSource.FromFile(ViewModel.SystemUser.UserImage);
                        imgClinician.Source = ImageSource.FromStream(() =>
                        {
                            var stream = file.GetStream();
                            return stream;
                        });
                        ViewModel.SaveFile(filename, file.GetStream());
                    }
                    else
                    {
                        await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera).ContinueWith(async (arg) =>
                        {
                            if (arg.IsCompleted)
                            {
                                var state = PermissionStatus.Unknown;
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

        void EditDetails(object sender, EventArgs e)
        {
            DisplayProfileDetails.IsVisible = false;
            EditProfileDetails.IsVisible = true;
        }

        void UpdateDetails(object sender, EventArgs e)
        {
            Update_Profile();

            DisplayProfileDetails.IsVisible = true;
            EditProfileDetails.IsVisible = false;

            /* scroll to top */
            ProfileScrollView.ScrollToAsync(ClinNameInput.Y, 0, true);
        }

        /* cancel update details */
        void CancelUpdate(object sender, EventArgs e)
        {
            DisplayProfileDetails.IsVisible = true;
            EditProfileDetails.IsVisible = false;

            /* scroll to top */
            ProfileScrollView.ScrollToAsync(ClinNameInput.Y, 0, true);
        }
    }
}
