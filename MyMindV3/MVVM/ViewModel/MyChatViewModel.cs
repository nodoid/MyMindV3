using MvvmFramework.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using MvvmFramework.Models;

namespace MvvmFramework.ViewModel
{
    public class MyChatViewModel : BaseViewModel
    {
        public MyChatViewModel()
        {
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
                await Send.SendData("api/MyMind/GetCometChatUserId", new List<string> { "userguid", guid, "authtoken", SystemUser.APIToken, "accounttype", GetIsClinician ? "3" : "2" }.ToArray()).ContinueWith((t) =>
                  {
                      if (t.IsCompleted)
                          UserId = t.Result;
                  });
            });
        }
    }
}
