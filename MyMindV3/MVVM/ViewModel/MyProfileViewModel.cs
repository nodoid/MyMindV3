using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
using MvvmFramework.Helpers;
using System.IO;
using System.Threading.Tasks;
using MvvmFramework.Models;
using System.Collections.Generic;
using MvvmFramework.Interfaces;
using System;
using Newtonsoft.Json.Schema;
using System.Linq;

namespace MvvmFramework.ViewModel
{
    public class MyProfileViewModel : BaseViewModel
    {
        INavigationService navService;
        IRepository sqlConn;
        IConnectivity connectService;
        IDialogService diaService;
        IUserSettings settingsService;

        public MyProfileViewModel(INavigationService nav, IRepository repo, IConnectivity con, IDialogService dia, IUserSettings settings)
        {
            navService = nav;
            sqlConn = repo;
            connectService = con;
            diaService = dia;
            settingsService = settings;

            if (con.IsConnected)
                SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Profile_Page_View :
                    (SystemUser.IsAuthenticated == 2 ? ActionCodes.User_Profile_Page_View : ActionCodes.Member_Profile_Page_View));

            GetUserProfile();
        }

        string filename;
        public string ImageFilename
        {
            get { return filename; }
            set { Set(() => ImageFilename, ref filename, value); }
        }

        public Stream GetProfileImage
        {
            get
            {
                GetData.GetImage(ImageFilename, IsUser).ConfigureAwait(true);
                settingsService.SaveSetting<string>("UserImageUpdate", SystemUser.ProfilePictureUploadTimestamp.ToString(), SettingType.String);
                return new FileIO().LoadFile(ImageFilename).Result;
            }
        }

        public bool LoadNewProfileImage
        {
            get
            {
                var rv = false;
                var dt = settingsService.LoadSetting<string>("UserImageUpdate", SettingType.String);
                if (string.IsNullOrEmpty(dt))
                    rv = true;
                else
                    if (Convert.ToDateTime(dt) != SystemUser.ProfilePictureUploadTimestamp)
                    rv = true;

                return rv;
            }
        }

        UserProfile userProfile;
        public UserProfile UserProfile
        {
            get { return userProfile; }
            set { Set(() => UserProfile, ref userProfile, value, true); }
        }

        string profName;
        public string ProfName
        {
            get { return profName; }
            set { Set(() => ProfName, ref profName, value, true); }
        }

        string profDOB;
        public string ProfDOB
        {
            get { return profDOB; }
            set { Set(() => ProfDOB, ref profDOB, value, true); }
        }

        string profPhone;
        public string ProfPhone
        {
            get { return profPhone; }
            set { Set(() => ProfPhone, ref profPhone, value, true); }
        }

        string profRefReason;
        public string ProfRefReason
        {
            get { return profRefReason; }
            set { Set(() => ProfRefReason, ref profRefReason, value, true); }
        }

        string profLikes;
        public string ProfLikes
        {
            get { return profLikes; }
            set { Set(() => ProfLikes, ref profLikes, value, true); }
        }

        string profDislikes;
        public string ProfDislikes
        {
            get { return profDislikes; }
            set { Set(() => ProfDislikes, ref profDislikes, value, true); }
        }

        string profGoals;
        public string ProfGoals
        {
            get { return profGoals; }
            set { Set(() => ProfGoals, ref profGoals, value, true); }
        }

        string profPrefName;
        public string ProfPrefName
        {
            get { return profPrefName; }
            set { Set(() => ProfPrefName, ref profPrefName, value, true); }
        }

        void GetUserProfile()
        {
            var usr = new UserProfile
            {
                APIToken = SystemUser.APIToken,
                APITokenExpiry = SystemUser.APITokenExpiry,
                Name = SystemUser.Name,
                DateOfBirth = SystemUser.DateOfBirth,
                ContactNumber = SystemUser.Phone,
                WhatIDo = SystemUser.Goals,
                ReferralReason = SystemUser.ReferralReason,
                Likes = SystemUser.Likes,
                Dislikes = SystemUser.Dislikes,
                PreferredName = SystemUser.PreferredName
            };
            UserProfile = usr;

            ProfName = usr.Name;
            ProfDOB = usr.DateOfBirth;
            ProfPhone = usr.ContactNumber;
            ProfRefReason = usr.ReferralReason;
            ProfLikes = usr.Likes;
            ProfDislikes = usr.Dislikes;
            ProfGoals = usr.WhatIDo;
            ProfPrefName = usr.PreferredName;
        }

        public void UpdateUserData(params string[] data)
        {
            Task.Run(async () =>
            {
                if (connectService.IsConnected)
                {
                    await Send.SendData("api/MyMind/UpdateUserProfile", "UserGUID", SystemUser.Guid, "AuthToken", SystemUser.APIToken,
                                        "PreferredName", data[2], "DateOfBirth", DateTime.Now.ToString(),
                                        "PhoneNumber", data[3], "WhyIThinkIWasReferred", data[4],
                                        "SomethingILike", data[5], "SomethingIDislike", data[6],
                                        "WhatIWantTo", data[7]).ContinueWith((t) =>
                 {
                     if (t.IsCompleted)
                     {
                         UpdateSystemUser();
                     }
                 });
                }
                else
                    await diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
            });
            var usr = UserProfile;
            usr.PreferredName = data[2];
            usr.ContactNumber = data[3];
            usr.ReferralReason = data[4];
            usr.Likes = data[5];
            usr.Dislikes = data[6];
            usr.WhatIDo = data[7];
            UserProfile = usr;
        }

        public void UpdateSystemUser()
        {
            if (!GetIsClinician && connectService.IsConnected)
                SendTrackingInformation(ActionCodes.User_Updated_Profile);

            SystemUser.PreferredName = UserProfile.Name;
            SystemUser.ContactNumber = UserProfile.ContactNumber;
            SystemUser.Goals = UserProfile.WhatIDo;
            SystemUser.ReferralReason = UserProfile.WhatIDo;
            SystemUser.Likes = UserProfile.Likes;
            SystemUser.Dislikes = UserProfile.Dislikes;
            SystemUser = SystemUser;
            sqlConn.SaveData(SystemUser);
        }

        public void SaveFile(string filename, Stream stream)
        {
            Task.Run(async () =>
            {
                await new FileIO().SaveFile(filename, stream);
            });
        }

        public void SetProfileDateTime(string dts)
        {
            var dt = dts.ConvertToDateTime();
            settingsService.SaveSetting<string>("UserImageUpdate", dt.ToString(), SettingType.String);
        }
    }
}
