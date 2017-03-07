using System;
using Xamarin.Forms;
using Plugin.Media;
using MyMindV3.Languages;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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
    public partial class MyProfileView : ContentPage
    {
        MyProfileViewModel ViewModel => App.Locator.MyProfile;
        MediaFile file;

        public MyProfileView()
        {
            InitializeComponent();
            BindingContext = ViewModel.SystemUser;
            CreateUI();
        }

        void CreateUI()
        {
            ViewModel.IsConnected = App.Self.IsConnected;
            DisplayProfileDetails.IsVisible = true;
            EditProfileDetails.IsVisible = false;
            GetImage();
            GetDetails().ConfigureAwait(true);
            if (ViewModel.SystemUser.IsAuthenticated != 3)
                CreateImageClick();

            vwRefresh.IsPullToRefreshEnabled = true;
            vwRefresh.RefreshColor = Color.Blue;

            vwRefresh.RefreshCommand = new Command(async () =>
            {
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
            var f = ViewModel.GetUserImage;
            if (ViewModel.SystemUser.IsAuthenticated == 3)
                imgUser.Source = "male_female.png";
            else
            {
                ViewModel.ImageFilename = ViewModel.Filename = ViewModel.GetUserImage;
                ViewModel.IsUser = ViewModel.SystemUser.IsAuthenticated == 2 ? true : false;
                if (ViewModel.FileExists)
                    imgUser.Source = ImageSource.FromFile(string.Format("{0}/{1}", ViewModel.GetCurrentFolder, ViewModel.GetUserImage));
                else
                    imgUser.Source = !string.IsNullOrEmpty(f) ? ImageSource.FromStream(() => ViewModel.GetProfileImage) : "male_female.png";
            }
        }

        async Task GetDetails()
        {
            await Send.SendData<List<UserProfile>>("api/MyMind/GetConnectionsProfile", "ClinicianGUID", ViewModel.ClinicianUser.ClinicianGUID, "AuthToken", ViewModel.ClinicianUser.APIToken,
                                                   "ClientGUID", ViewModel.SystemUser.Guid).ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        var ss = t.Result[0];
                        if (ss != null)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                ProfilePreferredNameInput.Text = ss.Name;
                                ProfileDoBInput.Text = ss.DateOfBirth;
                                ProfilePhoneInput.Text = ss.ContactNumber;
                                RegPhoneInput.Text = ss.ReferralReason;
                                ProfileLikesInput.Text = ss.Likes;
                                ProfileDislikesInput.Text = ss.Dislikes;
                                ProfileGoalsInput.Text = ss.Goals;
                            });
                        }
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

                        App.Self.UserSettings.SaveSetting(ViewModel.GetCurrentFolder, file.Path.Substring(0, file.Path.LastIndexOf('/')), SettingType.String);

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
                                            DependencyService.Get<IContent>().StoreFile(ViewModel.SystemUser.Guid, file.GetStream());
                                            ViewModel.SystemUser.UserImage = file.Path;
                                            ViewModel.UpdateSystemUser();
                                            //Send.HttpPost(file, _rootVM.SystemUser.Guid);
                                            await Send.UploadPicture(file.Path, ViewModel.SystemUser.Guid, ViewModel.SystemUser.APIToken);
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
            imgUser.GestureRecognizers.Add(imgTap);
        }

        void Update_Profile()
        {
            var res = Send.SendData("api/MyMind/UpdateUserProfile", "UserGUID", ViewModel.SystemUser.Guid, "AuthToken", ViewModel.SystemUser.APIToken,
                                    "PreferredName", EditProfilePreferredNameInput.Text, "DateOfBirth", EditProfileDoB.Date.ToString("g"),
                                    "PhoneNumber", EditProfilePhoneInput.Text, "WhyIThinkIWasReferred", EditRegPhoneInput.Text,
                                    "SomethingILike", EditProfileLikesInput.Text, "SomethingIDislike", EditProfileDislikesInput.Text,
                                    "WhatIWantTo", EditProfileGoalsInput.Text).ContinueWith((t) =>
             {
                 if (t.IsCompleted)
                 {
                     ViewModel.UpdateSystemUser();
                     var m = t.Result;
                 }
             });
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
            // save data back to internal - eventually send back to server for update
            ViewModel.UpdateSystemUser();

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
