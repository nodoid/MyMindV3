using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
using MvvmFramework.Helpers;
using MvvmFramework.Models;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MvvmFramework.ViewModel
{
    public class MyClinicianViewModel : BaseViewModel
    {
        INavigationService navService;
        IRepository sqlConn;
        public MyClinicianViewModel(INavigationService nav, IRepository repo)
        {
            navService = nav;
            sqlConn = repo;
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Profile_Page_View : ActionCodes.User_My_Clinician_Page_View);
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

        ClinicianProfile clinician;
        public ClinicianProfile Clinician
        {
            get { return clinician; }
            set { Set(() => Clinician, ref clinician, value, true); }
        }

        public void UpdateProfile(string whatIDo, string funFact, string contact)
        {
            Task.Run(async () =>
            {
                await Send.SendData("api/MyMind/UpdateClinicianProfile", "ClinicianGUID", Clinician.ClinicianGUID,
                "AuthToken", Clinician.APIToken,
                                    "WhatIDo", whatIDo, "FunFact", funFact, "ContactNumber", contact).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    UpdateSystemUser();
                }
            });
            });
        }

        public void GetClinicianDetails()
        {
            Task.Run(async () =>
            {
                await Send.SendData<List<ClinicianProfile>>("api/MyMind/GetClinicianProfile", "ClinicianGUID",
                    ClinicianUser.ClinicianGUID, "AuthToken", SystemUser.APIToken).ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        Clinician = t.Result[0];
                    }
                });
            });
        }

        public void UpdateSystemUser()
        {
            SendTrackingInformation(ActionCodes.Clinician_Updated_Profile);
            sqlConn.SaveData<SystemUser>(SystemUser);
        }
    }
}
