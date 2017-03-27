using System;
using AndroidView = Android.Views.View;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MyMindV3;
using MyMindV3.Droid;
using Com.Inscripts.Utils;
using Com.Orm;
using Com.Inscripts.Cometchat.Sdk;
using MvvmFramework;
using Org.Apache.Http.Authentication;

[assembly: ExportRenderer(typeof(CustomChatView), typeof(CustomChatViewRenderer))]
namespace MyMindV3.Droid
{
    public class CustomChatViewRenderer : ViewRenderer<CustomChatView, AndroidView>
    {
        CustomChatView chatview;
        protected override void OnElementChanged(ElementChangedEventArgs<CustomChatView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                chatview = e.NewElement as CustomChatView;
                LocalConfig.Initialize(Forms.Context);
                SugarContext.Init(Forms.Context);

                //MessageSDK.LaunchChatWindow(Forms.Context, e.OldElement.Username, new LoginCallback(successObj => MessageSDK.LaunchCometChat(MainActivity.Activity, null), failObj => MessagingCenter.Send(this, "CometChat", "Fail"), null, null, null, null));
                //MessageSDK.LaunchChatWindow(true, MainActivity.Activity, chatview.Username, new LoginCallback(successObj => MessageSDK.LaunchCometChat(MainActivity.Activity, null), failObj => { Console.WriteLine("failed"); MessagingCenter.Send(this, "CometChat", "Fail"); }, null, null, null, null));
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Username")
            {
                try
                {
                    MessageSDK.SetUrl(Forms.Context, Constants.CometChatURL, new ChatCallback(
                       successObj => MessageSDK.Login(Forms.Context, chatview.Username, "myminddefaultpassword",
                                                      new LoginCallback
                                                      (loginSuccess => MessageSDK.LaunchChatWindow(true, MainActivity.Activity, chatview.Username,
                                                                                                   new ChatCallback(launchCallback => MessageSDK.LaunchCometChat(MainActivity.Activity, null), failObj => Console.WriteLine("chat failed"), null, null, null, null)),
                                                       failObj => Console.WriteLine("launch failed"), null, null, null, null)),
                                                      failObj => { Console.WriteLine("login failed"); MessagingCenter.Send(this, "CometChat", "Fail"); }, null, null, null, null));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("launch exception {0}--{1}", ex.Message, ex.InnerException);
                }
            }
        }
    }
}
