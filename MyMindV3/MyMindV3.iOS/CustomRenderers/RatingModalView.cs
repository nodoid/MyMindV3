using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using MyMindV3;
using MyMindV3.iOS;

[assembly: ExportRenderer(typeof(ModalView), typeof(RatingModalView))]
namespace MyMindV3.iOS
{
    public class RatingModalView : ViewRenderer<ModalView, UIView>
    {
        int initialRating { get; set; }

        protected override void OnElementChanged(ElementChangedEventArgs<ModalView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var dialog = e.NewElement;
                initialRating = e.NewElement.Rating;
                CreateDialog(dialog);
            }
        }

        UIImageView oneStarImage, twoStarImage, threeStarImage, fourStarImage, fiveStarImage;
        void CreateDialog(ModalView dialog)
        {
            var currentRating = initialRating;
            var view = new UIView(new CGRect(8, 32, AppDelegate.Self.ScreenX, 175));
            view = UICreation.MakePrettyView(view);

            var shifter = AppDelegate.Self.ScreenX == 320 ? 0 : (AppDelegate.Self.ScreenX == 375 ? 27 : 48);
            var starShifter = AppDelegate.Self.ScreenX == 320 ? 0 : (AppDelegate.Self.ScreenX == 375 ? 52 : 76);
            var lblTitle = new UILabel(new CGRect(74 + shifter, 20, 153, 21))
            {
                Text = "Rate this resource",
                TextAlignment = UITextAlignment.Center
            };
            view.Add(lblTitle);

            oneStarImage = new UIImageView(new CGRect(18 + starShifter, 61, 40, 40))
            {
                Image = UIImage.FromBundle("emptystar.png"),
                UserInteractionEnabled = true
            };
            var oneTap = new UITapGestureRecognizer(() => { currentRating = 1; SetStarsUpto(currentRating); });
            oneStarImage.AddGestureRecognizer(oneTap);

            twoStarImage = new UIImageView(new CGRect(63 + starShifter, 61, 40, 40))
            {
                Image = UIImage.FromBundle("emptystar.png"),
                UserInteractionEnabled = true
            };
            var twoTap = new UITapGestureRecognizer(() => { currentRating = 2; SetStarsUpto(currentRating); });
            twoStarImage.AddGestureRecognizer(twoTap);

            threeStarImage = new UIImageView(new CGRect(108 + starShifter, 61, 40, 40))
            {
                Image = UIImage.FromBundle("emptystar.png"),
                UserInteractionEnabled = true
            };
            var threeTap = new UITapGestureRecognizer(() => { currentRating = 3; SetStarsUpto(currentRating); });
            threeStarImage.AddGestureRecognizer(threeTap);

            fourStarImage = new UIImageView(new CGRect(153 + starShifter, 61, 40, 40))
            {
                Image = UIImage.FromBundle("emptystar.png"),
                UserInteractionEnabled = true
            };
            var fourTap = new UITapGestureRecognizer(() => { currentRating = 4; SetStarsUpto(currentRating); });
            fourStarImage.AddGestureRecognizer(fourTap);

            fiveStarImage = new UIImageView(new CGRect(198 + starShifter, 61, 40, 40))
            {
                Image = UIImage.FromBundle("emptystar.png"),
                UserInteractionEnabled = true
            };
            var fiveTap = new UITapGestureRecognizer(() => { currentRating = 5; SetStarsUpto(currentRating); });
            fiveStarImage.AddGestureRecognizer(fiveTap);

            var btnClear = UICreation.CreateButton(new CGRect(39 + shifter, 119, 46, 30), UIButtonType.RoundedRect, "Clear");
            var btnCancel = UICreation.CreateButton(new CGRect(117 + shifter, 119, 63, 30), UIButtonType.RoundedRect, "Cancel");
            var btnDone = UICreation.CreateButton(new CGRect(215 + shifter, 119, 46, 30), UIButtonType.RoundedRect, "Done");

            view.AddSubviews(new UIView[] { oneStarImage, twoStarImage, threeStarImage, fourStarImage, fiveStarImage, btnDone, btnClear, btnCancel });

            btnClear.TouchUpInside += delegate
            {
                twoStarImage.Image = threeStarImage.Image = fourStarImage.Image = fiveStarImage.Image = UIImage.FromBundle("emptystar.png");
                currentRating = 1;
            };

            btnCancel.TouchUpInside += delegate
            {
                view.RemoveFromSuperview();
            };

            btnDone.TouchUpInside += delegate
            {
                App.Self.NewIconRating = currentRating;
                view.RemoveFromSuperview();
            };

            SetStarsUpto(currentRating);

            UIApplication.SharedApplication.KeyWindow.RootViewController.Add(view);
        }

        void SetStarsUpto(int rating)
        {
            var imageNames = new List<string> { "greenstar.png", "yellowstar.png", "orangestar.png", "purplestar.png", "bluestar.png" };
            var images = new List<UIImageView> { oneStarImage, twoStarImage, threeStarImage, fourStarImage, fiveStarImage };
            for (var i = 0; i < rating; ++i)
                images[i].Image = UIImage.FromBundle(imageNames[i]);
            if (rating < 5)
            {
                for (var n = rating; n < 5; ++n)
                    images[n].Image = UIImage.FromBundle("emptystar.png");
            }
        }
    }
}