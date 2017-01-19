using MyMindV3.Data;
using MyMindV3.ViewModels;
using System;
using Xamarin.Forms;
using Plugin.Media;
using MyMindV3.Languages;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Collections.Generic;
using MyMindV3.Models;
using System.Threading.Tasks;
#if DEBUG
using System.Diagnostics;
using System.Windows.Input;
#endif


namespace MyMindV3.Views
{
    public partial class MyProfileView : ContentPage
    {
        private RootViewModel _rootVM;
        private SystemUserDB _database;
        MediaFile file;

        public MyProfileView(RootViewModel incRootVM)
        {
            InitializeComponent();
            RootVM = incRootVM;
            BindingContext = RootVM.SystemUser;
            DisplayProfileDetails.IsVisible = true;
            EditProfileDetails.IsVisible = false;
            GetImage();
            GetDetails().ConfigureAwait(true);
            if (RootVM.SystemUser.IsAuthenticated != 3)
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
            var f = _rootVM.SystemUser.UserImage;
            if (RootVM.SystemUser.IsAuthenticated == 3)
                imgUser.Source = "male_female.png";
            else
                imgUser.Source = !string.IsNullOrEmpty(f) ? f : "male_female.png";
        }

        async Task GetDetails()
        {
            await Send.SendData<List<UserProfile>>("api/MyMind/GetConnectionsProfile", "ClinicianGUID", RootVM.ClinicianUser.ClinicianGUID, "AuthToken", RootVM.ClinicianUser.APIToken,
                                                   "ClientGUID", RootVM.SystemUser.Guid).ContinueWith((t) =>
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
                            Name = string.Format("{0}.jpg", _rootVM.SystemUser.Guid),
                            CompressionQuality = 92,
                            PhotoSize = PhotoSize.Small
                        });

                        if (file == null)
                            return;

                        App.Self.UserSettings.SaveSetting("ImageDirectory", file.Path.Substring(0, file.Path.LastIndexOf('/')), SettingType.String);

                        try
                        {
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
            var res = Send.SendData("api/MyMind/UpdateUserProfile", "UserGUID", RootVM.SystemUser.Guid, "AuthToken", RootVM.SystemUser.APIToken,
                                    "PreferredName", EditProfilePreferredNameInput.Text, "DateOfBirth", EditProfileDoB.Date.ToString("g"),
                                    "PhoneNumber", EditProfilePhoneInput.Text, "WhyIThinkIWasReferred", EditRegPhoneInput.Text,
                                    "SomethingILike", EditProfileLikesInput.Text, "SomethingIDislike", EditProfileDislikesInput.Text,
                                    "WhatIWantTo", EditProfileGoalsInput.Text).ContinueWith((t) =>
             {
                 if (t.IsCompleted)
                 {
                     var userProfile = _database.RegisterWeb(RootVM.SystemUser);
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
            _database = new SystemUserDB();

            _database.UpdateSystemUser(_rootVM.SystemUser);

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
