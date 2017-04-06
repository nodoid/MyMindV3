using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using MvvmFramework.Models;
using MvvmFramework.Interfaces;

namespace MvvmFramework.ViewModel
{
    public class MyPatientViewModel : BaseViewModel
    {
        IConnectivity connectService;
        IDialogService diaService;

        public MyPatientViewModel(IConnectivity con, IDialogService dia)
        {
            connectService = con;
            diaService = dia;
        }

        List<Connections> connections;
        public List<Connections> Connections
        {
            get { return connections; }
            set { Set(() => Connections, ref connections, value); }
        }

        List<string> connectionNames;
        public List<string> ConnectionNames
        {
            get { return connectionNames; }
            set { Set(() => ConnectionNames, ref connectionNames, value, true); }
        }

        string clinicianGuid;
        public string ClinicianGuid
        {
            get { return clinicianGuid; }
            set { Set(() => ClinicianGuid, ref clinicianGuid, value, true); }
        }

        string authToken;
        public string AuthToken
        {
            get { return authToken; }
            set { Set(() => AuthToken, ref authToken, value); }
        }

        string unencyptedGuid;
        public string UnencryptedGuid
        {
            get { return unencyptedGuid; }
            set { Set(() => UnencryptedGuid, ref unencyptedGuid, value); }
        }

        string unencryptedAuth;
        public string UnencryptedAuth
        {
            get { return unencryptedAuth; }
            set { Set(() => UnencryptedAuth, ref unencryptedAuth, value); }
        }

        UserProfile userProfile;
        public UserProfile UserProfile
        {
            get { return userProfile; }
            set { Set(() => UserProfile, ref userProfile, value, true); }
        }

        string connectionId;
        public string ConnectionId
        {
            get { return connectionId; }
            set { Set(() => ConnectionId, ref connectionId, value); }
        }

        public void GetUserProfile()
        {
            Task.Run(async () =>
            {
                if (connectService.IsConnected)
                {
                    await Send.SendData<List<UserProfile>>("api/MyMind/GetConnectionsProfile", "ClinicianGUID",
                                                           ClinicianUser.ClinicianGUID, "AuthToken", SystemUser.APIToken,
                                                           "ClientGUID", ConnectionId).ContinueWith((t) =>
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

        public void GetConnections()
        {
            Task.Run(async () =>
            {
                if (connectService.IsConnected)
                {
                    await Send.SendData<List<Connections>>("api/MyMind/GetClinicianConnections", "ClinicianGUID",
                                                           ClinicianUser.ClinicianGUID, "AuthToken", SystemUser.APIToken).ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        Connections = t.Result;
                        if (t.Result.Count != 0)
                        {
                            ConnectionNames = t.Result.Select(w => w.Name).ToList();
                        }
                    }
                });
                }
                else
                    await diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
            });
        }
    }
}
