using System;
using Xamarin.Forms;
using Plugin.Media;
using MyMindV3.Languages;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Collections.Generic;
using MvvmFramework.ViewModel;
using MvvmFramework;
using System.Linq;
#if DEBUG
using System.Diagnostics;
#endif

namespace MyMindV3.Views
{
    public partial class MyProfileView : ContentPage
    {
        MyProfileViewModel ViewModel => App.Locator.MyProfile;
        MediaFile file;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "UserProfile")
                {
                    var ss = ViewModel.UserProfile;
                    if (ss != null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ProfilePreferredNameInput.Text = ss.PreferredName;
                            ProfileDoBInput.Text = ss.DateOfBirth;
                            ProfilePhoneInput.Text = ss.ContactNumber;
                            RegPhoneInput.Text = ss.ReferralReason;
                            ProfileLikesInput.Text = ss.Likes;
                            ProfileDislikesInput.Text = ss.Dislikes;
                            ProfileGoalsInput.Text = ss.Goals;
                        });
                    }
                }
            };
        }

        public MyProfileView()
        {
            InitializeComponent();
            //BindingContext = ViewModel.UserProfile;
            BindingContext = ViewModel;
            CreateUI();
        }

        void CreateUI()
        {
            DisplayProfileDetails.IsVisible = true;
            EditProfileDetails.IsVisible = false;
            GetImage();

            if (ViewModel.SystemUser.IsAuthenticated != 3)
                CreateImageClick();

            vwRefresh.IsPullToRefreshEnabled = true;
            vwRefresh.RefreshColor = Color.Blue;

            vwRefresh.RefreshCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(() => { GetImage(); vwRefresh.IsRefreshing = false; });
            });
        }

        void GetImage()
        {
            var f = ViewModel.GetUserImage;

            ViewModel.ImageFilename = ViewModel.Filename = string.Format("{0}/{1}.jpg", ViewModel.GetCurrentFolder, ViewModel.GetUserImage);
            ViewModel.IsUser = ViewModel.SystemUser.IsAuthenticated == 2 ? true : false;
            if (ViewModel.FileExists && !ViewModel.LoadNewProfileImage)
                Device.BeginInvokeOnMainThread(() => imgUser.Source = ImageSource.FromFile(ViewModel.ImageFilename));
            else
                Device.BeginInvokeOnMainThread(() => imgUser.Source = !string.IsNullOrEmpty(ViewModel.ImageFilename) ? ImageSource.FromStream(() => ViewModel.GetProfileImage) : "male_female.png");
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
                            //Directory = "Directory",
                            Name = string.Format("{0}.jpg", ViewModel.SystemUser.Guid),
                            CompressionQuality = 92,
                            PhotoSize = PhotoSize.Small
                        });

                        if (file == null)
                            return;

                        //App.Self.UserSettings.SaveSetting(ViewModel.GetCurrentFolder, file.Path.Substring(0, file.Path.LastIndexOf('/')), SettingType.String);

                        var filename = string.Format("{0}/{1}", ViewModel.GetCurrentFolder, file.Path.Split('/').ToList().Last());
                        if (filename.Contains("_"))
                        {
                            var fn = filename.Split('_');
                            filename = fn[0] + ".jpg";
                        }

                        try
                        {
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
                                            ViewModel.SystemUser.UserImage = file.Path;
                                            ViewModel.UpdateSystemUser();
                                            if (App.Self.IsConnected)
                                                await Send.UploadPicture(file.GetStream(), file.Path, ViewModel.SystemUser.Guid, ViewModel.SystemUser.APIToken, ViewModel.SystemUser.IsAuthenticated.ToString()).ContinueWith((z) =>
                                            {
                                                if (z.IsCompleted && (!z.IsFaulted || !z.IsCanceled))
                                                {
                                                    if (z.Result != "-1")
                                                        ViewModel.SetProfileDateTime(z.Result);
                                                }
                                            });
                                            else
                                                await DisplayAlert(Langs.Network_ErrorTitle, Langs.Network_ErrorMessage, Langs.Gen_OK);
                                            file.Dispose();
                                        }
                                    }));
                        }
                        catch (Exception e)
                        {
#if DEBUG
                            Debug.WriteLine("Upload exception {0}--{1}", e.Message, e.InnerException);
#endif
                        }
                        imgUser.Source = ImageSource.FromStream(() =>
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
            imgUser.GestureRecognizers.Add(imgTap);
        }

        void Update_Profile()
        {
            ViewModel.UpdateUserData(new List<string>
            {
                ViewModel.SystemUser.Guid, ViewModel.SystemUser.APIToken,EditProfilePreferredNameInput.Text,
                EditProfilePhoneInput.Text,EditRegPhoneInput.Text,
                EditProfileLikesInput.Text,EditProfileDislikesInput.Text,EditProfileGoalsInput.Text
            }.ToArray());
        }

        void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }

        /* switch between display/edit profile details */
        private void EditDetails(object sender, EventArgs e)
        {
            DisplayProfileDetails.IsVisible = false;
            EditProfileDetails.IsVisible = true;
        }

        /* switch between display/edit profile details */
        private void UpdateDetails(object sender, EventArgs e)
        {
            Update_Profile();

            var ss = ViewModel.UserProfile;
            if (ss != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ProfilePreferredNameInput.Text = ss.PreferredName;
                    ProfileDoBInput.Text = ss.DateOfBirth;
                    ProfilePhoneInput.Text = ss.ContactNumber;
                    RegPhoneInput.Text = ss.ReferralReason;
                    ProfileLikesInput.Text = ss.Likes;
                    ProfileDislikesInput.Text = ss.Dislikes;
                    ProfileGoalsInput.Text = ss.WhatIDo;
                });
            }

            DisplayProfileDetails.IsVisible = true;
            EditProfileDetails.IsVisible = false;

            /* scroll to top */
            ProfileScrollView.ScrollToAsync(ProfileNameInput.Y, 0, true);
        }

        /* cancel update details */
        private void CancelUpdate(object sender, EventArgs e)
        {
            DisplayProfileDetails.IsVisible = true;
            EditProfileDetails.IsVisible = false;

            /* scroll to top */
            ProfileScrollView.ScrollToAsync(ProfileNameInput.Y, 0, true);
        }
    }
}
