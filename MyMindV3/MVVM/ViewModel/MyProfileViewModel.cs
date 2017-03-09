using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
using MvvmFramework.Helpers;
using System.IO;
using System.Threading.Tasks;
using MvvmFramework.Models;
using System.Collections.Generic;

namespace MvvmFramework.ViewModel
{
    public class MyProfileViewModel : BaseViewModel
    {
        INavigationService navService;
        IRepository sqlConn;

        public MyProfileViewModel(INavigationService nav, IRepository repo)
        {
            navService = nav;
            sqlConn = repo;
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Profile_Page_View :
                (SystemUser.IsAuthenticated == 2 ? ActionCodes.User_Profile_Page_View : ActionCodes.Member_Profile_Page_View));
        }

        string filename;
        public string ImageFilename
        {
            get { return filename; }
            set { Set(() => ImageFilename, ref filename, value); }
        }

        public Stream GetProfileImage
        {
            get {
                GetData.GetImage(ImageFilename, IsUser).ConfigureAwait(true);
                return new FileIO().LoadFile(ImageFilename).Result;
            }
        }

        UserProfile userProfile;
        public UserProfile UserProfile
        {
            get { return userProfile; }
            set { Set(() => UserProfile, ref userProfile, value, true); }
        }

        public void GetUserProfileDetails(params string[] data)
        {
            Task.Run(async () =>
            {
                await Send.SendData<List<UserProfile>>("api/MyMind/GetConnectionsProfile", "ClinicianGUID", data[0], "AuthToken", data[1],
                                                   "ClientGUID", data[2]).ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        UserProfile = t.Result[0];
                    }
                });
            });
        }

        public void UpdateUserData(params string[] data)
        {
            Task.Run(async () =>
            {
                await Send.SendData("api/MyMind/UpdateUserProfile", "UserGUID", data[0], "AuthToken", data[1],
                                    "PreferredName", data[2], "DateOfBirth", data[3],
                                    "PhoneNumber", data[4], "WhyIThinkIWasReferred", data[5],
                                    "SomethingILike", data[6], "SomethingIDislike", data[7],
                                    "WhatIWantTo", data[8]).ContinueWith((t) =>
             {
                 if (t.IsCompleted)
                 {
                     UpdateSystemUser();
                 }
             });
            });
        }

        public void UpdateSystemUser()
        {
            if (!GetIsClinician)
                SendTrackingInformation(ActionCodes.User_Updated_Profile);
            sqlConn.SaveData(SystemUser);
        }
    }
}
