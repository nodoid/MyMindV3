using System;
using MyMindV3.iOS;
using Xamarin.Forms;
using UIKit;
using System.Linq;
using Foundation;

[assembly: Dependency(typeof(LaunchChatService))]
namespace MyMindV3.iOS
{
    public class LaunchChatService : ILaunchCometChat
    {
        public void LaunchChat()
        {
            var messageSDK = new MessageSDK();
            var rootvc = UIApplication.SharedApplication.KeyWindow;
            var vc = rootvc.RootViewController?.ChildViewControllers.First().ChildViewControllers.Last().ChildViewControllers.First(); ;
            if (vc != null)
            {
                var navcontroller = vc as UINavigationController;
                messageSDK.LoginWithURL(Constants.CometChatURL, "emma", "clinician", navcontroller);
            }
        }

        private void OnSuccess(NSDictionary dict)
        {
            Console.WriteLine("OnSuccess : " + dict);
        }

        private void OnFailure(NSError err)
        {
            Console.WriteLine("OnFailure : " + err);
        }
    }
}
