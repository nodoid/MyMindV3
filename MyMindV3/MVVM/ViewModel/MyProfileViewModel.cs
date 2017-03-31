using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
using MvvmFramework.Helpers;
using System.IO;
using System.Threading.Tasks;
using MvvmFramework.Models;
using System.Collections.Generic;
using MvvmFramework.Interfaces;

namespace MvvmFramework.ViewModel
{
    public class MyProfileViewModel : BaseViewModel
    {
        INavigationService navService;
        IRepository sqlConn;
        IConnectivity connectService;
        IDialogService diaService;

        public MyProfileViewModel(INavigationService nav, IRepository repo, IConnectivity con, IDialogService dia)
        {
            navService = nav;
            sqlConn = repo;
            connectService = con;
            diaService = dia;

            if (con.IsConnected)
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
            get
            {
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
                if (connectService.IsConnected)
                {
                    await Send.SendData<List<UserProfile>>("api/MyMind/GetConnectionsProfile", "ClinicianGUID", data[0], "AuthToken", data[1],
                                                       "ClientGUID", data[2]).ContinueWith((t) =>
                    {
                        if (t.IsCompleted)
                        {
                            UserProfile = t.Result[0];
                        }
                    });
                }
                else
                    await diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
            });
        }

        public void UpdateUserData(params string[] data)
        {
            Task.Run(async () =>
            {
                if (connectService.IsConnected)
                {
                    await Send.SendData("api/MyMind/UpdateUserProfile", "UserGUID", data[0], "AuthToken", data[1],
                                        "PreferredName", data[2],
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
            if (usr == null)
                usr = new UserProfile();
            usr.Name = data[2];
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
            sqlConn.SaveData(SystemUser);
        }
    }
}
