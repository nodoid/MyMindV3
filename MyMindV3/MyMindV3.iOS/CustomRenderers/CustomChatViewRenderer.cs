using System;
using Foundation;
using MessageSDKBinding.iOS;
using MyMindV3;
using MyMindV3.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(CustomChatView), typeof(CustomChatViewRenderer))]
namespace MyMindV3.iOS
{
    public class CustomChatViewRenderer : ViewRenderer
    {
        MessageSDK messageSDK;
        CustomChatView chatView;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                chatView = e.NewElement as CustomChatView;

                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }

                messageSDK = new MessageSDK();
                //messageSDK.LoginWithURL("https://chat.phpchatsoftware.com", "demo1","user1", this, (dict) => { this.OnSuccess(dict); }, (dict1) => { this.UserInfo(dict1); }, (dict2) => { this.ChatroomInfo(dict2); }, (dict3) => { this.OnMessageReceive(dict3); }, (err) => { this.OnFailure(err); });
                messageSDK.LoginWithURL(MvvmFramework.Constants.CometChatURL, chatView.Username, vc, (dict) => { this.OnSuccess(dict); }, (dict1) => { this.UserInfo(dict1); }, (dict2) => { this.ChatroomInfo(dict2); }, (dict3) => { this.OnMessageReceive(dict3); }, (err) => { this.OnFailure(err); });
            }
        }

        private void OnSuccess(NSDictionary dict)
        {
            Console.WriteLine("OnSuccess : " + dict);
        }

        private void UserInfo(NSDictionary dict1)
        {
            Console.WriteLine("UserInfo : " + dict1);
        }

        private void ChatroomInfo(NSDictionary dict2)
        {
            Console.WriteLine("ChatroomInfo : " + dict2);
        }

        private void OnMessageReceive(NSDictionary dict3)
        {
            Console.WriteLine("OnMessageReceive : " + dict3);
        }

        private void OnFailure(NSError err)
        {
            Console.WriteLine("OnFailure : " + err);
        }
    }
}
