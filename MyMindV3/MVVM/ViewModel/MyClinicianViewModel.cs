using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
using MvvmFramework.Helpers;
using MvvmFramework.Models;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using MvvmFramework.Interfaces;

namespace MvvmFramework.ViewModel
{
    public class MyClinicianViewModel : BaseViewModel
    {
        INavigationService navService;
        IRepository sqlConn;
        IDialogService diaService;
        IConnectivity connectService;

        public MyClinicianViewModel(INavigationService nav, IRepository repo, IDialogService dia, IConnectivity con)
        {
            navService = nav;
            sqlConn = repo;
            diaService = dia;
            connectService = con;

            if (connectService.IsConnected)
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Profile_Page_View : ActionCodes.User_My_Clinician_Page_View);
        }

        string filename;
        public string ImageFilename
        {
            get { return filename; }
            set { Set(() => ImageFilename, ref filename, value, true); }
        }

        public Stream GetProfileImage
        {
            get
            {
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
                if (connectService.IsConnected)
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
                }
                else
                    await diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
            });
        }

        public void GetClinicianDetails()
        {
            Task.Run(async () =>
            {
                if (connectService.IsConnected)
                {
                    await Send.SendData<List<ClinicianProfile>>("api/MyMind/GetClinicianProfile", "ClinicianGUID",
                        ClinicianUser.ClinicianGUID, "AuthToken", SystemUser.APIToken).ContinueWith((t) =>
                    {
                        if (t.IsCompleted)
                        {
                            Clinician = t.Result[0];
                        }
                    });
                }
                else
                    await diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
            });
        }

        public void UpdateSystemUser()
        {
            if (connectService.IsConnected)
            {
                SendTrackingInformation(ActionCodes.Clinician_Updated_Profile);
            }
                sqlConn.SaveData<SystemUser>(SystemUser);
        }
    }
}
