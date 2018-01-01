using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
using MvvmFramework.Helpers;
using MvvmFramework.Models;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using MvvmFramework.Interfaces;
using System;
using System.Linq;

namespace MvvmFramework.ViewModel
{
    public class MyClinicianViewModel : BaseViewModel
    {
        INavigationService navService;
        IRepository sqlConn;
        IDialogService diaService;
        IConnectivity connectService;
        IUserSettings settingsService;

        public MyClinicianViewModel(INavigationService nav, IRepository repo, IDialogService dia, IConnectivity con, IUserSettings settings)
        {
            navService = nav;
            sqlConn = repo;
            diaService = dia;
            connectService = con;
            settingsService = settings;

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
                Stream data = null;
                Task.Run(async () =>
                {
                    await GetData.GetImage(ImageFilename, IsUser).ContinueWith(async (t) =>
                    {
                        if (t.IsCompleted && (!t.IsFaulted || !t.IsCanceled))
                        {
                            settingsService.SaveSetting<string>("ClinicianImageUpdate", ClinicianUser.ProfilePictureUploadTimestamp.ToString(), SettingType.String);
                            await new FileIO().LoadFile(ImageFilename).ContinueWith((w) =>
                            {
                                data = w.Result;
                            });
                        }
                    });
                });
                while (data == null)
                { }
                return data;
            }
        }

        public bool LoadNewProfileImage
        {
            get
            {
                var rv = false;
                var dt = settingsService.LoadSetting<string>("ClinicianImageUpdate", SettingType.String);
                if (string.IsNullOrEmpty(dt))
                    rv = true;
                else
                    if (Convert.ToDateTime(dt) != ClinicianUser.ProfilePictureUploadTimestamp)
                    rv = true;

                return rv;
            }
        }

        ClinicianProfile clinician;
        public ClinicianProfile Clinician
        {
            get { return clinician; }
            set { Set(() => Clinician, ref clinician, value, true); }
        }

        public void SaveFile(string filename, Stream stream)
        {
            Task.Run(async () =>
            {
                await new FileIO().SaveFile(filename, stream);
                //settingsService.SaveSetting<string>("ClinicianImageUpdate", ClinicianUser.ProfilePictureUploadTimestamp.ToString(), SettingType.String);
            });
        }

        public void UpdateProfile(string whatIDo, string funFact, string contact)
        {
            Task.Run(async () =>
            {
                if (connectService.IsConnected)
                {
                    await Send.SendData("api/MyMind/UpdateClinicianProfile", "ClinicianGUID", ClinicianUser.ClinicianGUID,
                                        "AuthToken", ClinicianUser.APIToken,
                                        "WhatIDo", whatIDo, "FunFact", funFact, "ContactNumber", contact).ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        Clinician.FunFact = funFact;
                        Clinician.ContactNumber = contact;
                        Clinician.WhatIDo = whatIDo;
                        Clinician = Clinician;
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
                            Name = Clinician.Name;
                            Role = Clinician.WhatIDo;
                            FunFact = Clinician.FunFact;
                            Phone = Clinician.ContactNumber;
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
            //sqlConn.SaveData<ClinicianUser>(ClinicianUser);
        }

        public void SetProfileDateTime(string dts)
        {
            var dt = dts.ConvertToDateTime();
            settingsService.SaveSetting<string>("ClinicianImageUpdate", dt.ToString(), SettingType.String);
        }

        string name;
        public string Name
        {
            get { return name; }
            set { Set(() => Name, ref name, value, true); }
        }

        string role;
        public string Role
        {
            get { return role; }
            set { Set(() => Role, ref role, value, true); }
        }

        string funfact;
        public string FunFact
        {
            get { return funfact; }
            set { Set(() => FunFact, ref funfact, value, true); }
        }

        string phone;
        public string Phone
        {
            get { return phone; }
            set { Set(() => Phone, ref phone, value, true); }
        }
    }
}
