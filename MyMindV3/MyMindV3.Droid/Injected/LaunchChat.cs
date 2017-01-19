using System;
using Com.Inscripts.Cometchat.Sdk;
using Com.Inscripts.Utils;
using Com.Orm;
using MyMindV3.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(LaunchChatService))]
namespace MyMindV3.Droid
{
    class LaunchChatService : ILaunchCometChat
    {
        public void LaunchChat()
        {
            //Forms.Context.StartActivity(typeof(CometChatActivity));
            //Forms.Context.StartActivity(typeof(ChatroomListActivity));
            try
            {
                LocalConfig.Initialize(Forms.Context);
                SugarContext.Init(Forms.Context);

                //MessageSDK.SetUrl(Forms.Context, Constants.CometChatURL, new MyCallback(success => MessageSDK.Login(Forms.Context, "member", "health",
                //new MyCallback(success2 => MessageSDK.LaunchChatWindow(true, MainActivity.Active, "member", null)))));

                MessageSDK.SetUrl(Forms.Context, "https://www.digitalroots.co.uk/", new MyCallback(success => MessageSDK.Login(Forms.Context, "user", "health",
                                                                                                                    new MyCallback(success2 => MessageSDK.LaunchChatWindow(true, MainActivity.Active, "user", null)))));
            }
            catch (Exception e)
            {
                App.Self.MessageEvents.BroadcastIt("Chat", e.Message);
            }
        }
    }
}