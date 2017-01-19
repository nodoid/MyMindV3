using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using CoreGraphics;
using MyMindV3.iOS;

[assembly: Dependency(typeof(NetworkSpin))]
namespace MyMindV3.iOS
{
    public class NetworkSpin : INetworkSpinner
    {
        static UIView spinView;
        static UILabel txtMessage;

        public void ChangeMessage(string newMessage)
        {
            if (spinView != null)
            {
                using (var pool = new NSAutoreleasePool())
                {
                    pool.InvokeOnMainThread(delegate ()
                    {
                        txtMessage.Text = newMessage;
                    });
                }
            }
        }

        public void NetworkSpinner(bool on, string title, string message)
        {
            var lblTitle = new UILabel(new CGRect(89, 7, 192, 26))
            {
                TextColor = UIColor.Black,
                Font = UIFont.SystemFontOfSize((nfloat)22),
                Text = title
            };

            var spinSpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge)
            {
                Frame = new CGRect(25, 19, 286, 75),
                AutoresizingMask = UIViewAutoresizing.All,
            };

            if (spinView == null)
            {
                var centerX = (App.ScreenSize.Width / 2) - (286 / 2);
                var centerY = (App.ScreenSize.Height / 2) - (75 / 2);
                spinView = new UIView(new CGRect(centerX, centerY, 286, 75))
                {
                    BackgroundColor = UIColor.LightGray,
                };
                spinView.Layer.BorderWidth = 0;
                spinView.Layer.CornerRadius = 4f;
                txtMessage = new UILabel(new CGRect(45, 41, 185, 21))
                {
                    Text = message
                };
                spinView.AddSubviews(new UIView[] { spinSpinner, lblTitle, txtMessage });
            }

            spinView.BringSubviewToFront(spinSpinner);
            UIApplication.SharedApplication.KeyWindow.RootViewController.Add(spinView);
            if (on)
                spinSpinner.StartAnimating();
            else
                spinView.RemoveFromSuperview();

            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate ()
                {
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = on;
                });
            }
        }
    }
}

