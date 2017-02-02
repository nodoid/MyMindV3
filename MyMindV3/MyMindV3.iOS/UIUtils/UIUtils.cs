using CoreGraphics;
using System;
using UIKit;

namespace MyMindV3.iOS
{
    public static class UICreation
    {
        public static UIButton CreateButton(CGRect rect, UIButtonType type, string title)
        {
            var tmp = new UIButton(type)
            {
                Frame = rect,
                BackgroundColor = UIColor.FromRGB(130, 186, 132),
            };
            tmp.Layer.BorderWidth = 0.8f;
            tmp.Layer.BorderColor = UIColor.FromRGB(45, 176, 51).CGColor;
            tmp.Layer.CornerRadius = 10f;
            tmp.SetTitle(title, UIControlState.Normal);
            return tmp;
        }

        public static UIView CreateDoubleLabelView(float y, string leftText, string rightText)
        {
            var quarter = AppDelegate.Self.ScreenX / 4;
            var labelLeft = new UILabel(new CGRect(8, 4, (3 * quarter) - 32, 21))
            {
                AdjustsFontSizeToFitWidth = true,
                TextColor = UIColor.Black,
                Text = leftText
            };
            var labelRight = new UILabel(new CGRect((3 * quarter) - 16, 4, quarter - 16, 21))
            {
                AdjustsFontSizeToFitWidth = true,
                TextColor = UIColor.Red,
                Text = rightText
            };

            var view = new UIView(new CGRect(4, y, AppDelegate.Self.ScreenX - 8, 90));
            view.AddSubviews(new UIView[] { labelLeft, labelRight });
            return view;
        }

        public static UIView MakePrettyView(UIView vwView, float high = 0, float x = 16f, float y = 80f)
        {
            vwView.Layer.CornerRadius = 4f;
            vwView.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 210);

            var divider = AppDelegate.Self.Retina ? 2 : 1;
            nfloat height = (nfloat)high;

            var width = vwView.Bounds.Width + x > (AppDelegate.Self.ScreenX / divider) ? vwView.Bounds.Width - (x * 2) : vwView.Frame.Width;
            if (high == 0f)
                height = vwView.Bounds.Height + y > (AppDelegate.Self.ScreenY / divider) ? vwView.Bounds.Height - (y * 2) : vwView.Frame.Height;

            if (!AppDelegate.Self.IsIPhone)
            {
                x = (float)AppDelegate.Self.ScreenX - ((float)width / 2);
            }
            vwView.Frame = new CGRect(8, y, AppDelegate.Self.IsIPhone ? AppDelegate.Self.ScreenX - 16 : width, height);
            vwView.Layer.BorderWidth = 1.4f;
            vwView.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            vwView.Layer.ShadowOpacity = 0.75f;
            return vwView;
        }
    }

}
