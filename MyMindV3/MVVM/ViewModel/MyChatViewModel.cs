using MvvmFramework.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using MvvmFramework.Models;
using MvvmFramework.Interfaces;
using GalaSoft.MvvmLight.Views;

namespace MvvmFramework.ViewModel
{
    public class MyChatViewModel : BaseViewModel
    {
        IConnectivity connectService;
        IDialogService diaService;
        
        public MyChatViewModel(IConnectivity con, IDialogService dia)
        {
            connectService = con;
            if (connectService.IsConnected)
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Chat_Page_View : ActionCodes.User_My_Chat_Page_View);
        }

        string userId;
        public string UserId
        {
            get { return userId; }
            set { Set(() => UserId, ref userId, value, true); }
        }

        public void GetUserId(string guid)
        {
            Task.Run(async () =>
            {
                if (connectService.IsConnected)
                {
                    await Send.SendData("api/MyMind/GetCometChatUserId", new List<string> { "userguid", guid, "authtoken", SystemUser.APIToken, "accounttype", GetIsClinician ? "3" : "2" }.ToArray()).ContinueWith((t) =>
                      {
                          if (t.IsCompleted)
                              UserId = t.Result;
                      });
                }
                else
                    await diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
            });
        }
    }
}
