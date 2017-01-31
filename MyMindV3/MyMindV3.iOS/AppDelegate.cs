using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using UIKit;
using Xamarin.Forms;

namespace MyMindV3.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public bool Retina { get; private set; }

        public nfloat ScreenX { get; private set; }

        public nfloat ScreenY { get; private set; }

        public bool IsIPhone { get; private set; }

        public static AppDelegate Self { get; private set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AppDelegate.Self = this;
            global::Xamarin.Forms.Forms.Init();
            ImageCircleRenderer.Init();
            PullToRefreshViewRenderer.Init();
            App.ScreenSize = new Size(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
            IsIPhone = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone;

            Retina = UIScreen.MainScreen.RespondsToSelector(new ObjCRuntime.Selector("scale")) ? true : false;

            ScreenX = UIScreen.MainScreen.Bounds.Width;
            ScreenY = UIScreen.MainScreen.Bounds.Height;
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
