
using Android.App;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Collections.Generic;
using MyMindV3;
using MyMindV3.Droid;

[assembly: ExportRenderer(typeof(ModalView), typeof(RatingModalView))]
namespace MyMindV3.Droid
{
    public class RatingModalView : ViewRenderer<ModalView, Android.Views.View>
    {
        int initialRating { get; set; } = 0;

        protected override void OnElementChanged(ElementChangedEventArgs<ModalView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var dialog = e.NewElement;
                initialRating = e.NewElement.Rating;
                dialog.HorizontalOptions = LayoutOptions.Center;
                dialog.VerticalOptions = LayoutOptions.Center;

                CreateDialog(dialog);
            }
        }

        ImageView imgBtn1, imgBtn2, imgBtn3, imgBtn4, imgBtn5, imgBtn6;

        public void CreateDialog(ModalView dialog)
        {
            var dispModal = new Dialog(MainActivity.Activity, Resource.Style.lightbox_dialog);
            dispModal.SetContentView(Resource.Layout.modalratings);
            var currentRating = initialRating;

            // create the links to the UI elements
            ((Android.Widget.Button)dispModal.FindViewById(Resource.Id.btnCancel)).Click += delegate
            {
                dispModal.Dismiss();
            };
            ((Android.Widget.Button)dispModal.FindViewById(Resource.Id.btnDone)).Click += delegate
            {
                App.Self.NewIconRating = currentRating;
                dispModal.Dismiss();
            };
            ((Android.Widget.Button)dispModal.FindViewById(Resource.Id.btnClear)).Click += delegate
            {
                currentRating = 0;
                UpdateStars(currentRating);
            };
            imgBtn1 = dispModal.FindViewById<ImageView>(Resource.Id.imageButton1);
            imgBtn2 = dispModal.FindViewById<ImageView>(Resource.Id.imageButton2);
            imgBtn3 = dispModal.FindViewById<ImageView>(Resource.Id.imageButton3);
            imgBtn4 = dispModal.FindViewById<ImageView>(Resource.Id.imageButton4);
            imgBtn5 = dispModal.FindViewById<ImageView>(Resource.Id.imageButton5);
            imgBtn6 = dispModal.FindViewById<ImageView>(Resource.Id.imageButton6);

            imgBtn1.Clickable = imgBtn2.Clickable = imgBtn3.Clickable = imgBtn4.Clickable = imgBtn5.Clickable = imgBtn6.Clickable = true;

            SetStartStars();

            imgBtn1.Click += delegate
            {
                currentRating = 1;
                UpdateStars(currentRating);
            };
            imgBtn2.Click += delegate
            {
                currentRating = 2;
                UpdateStars(currentRating);
            };
            imgBtn3.Click += delegate
            {
                currentRating = 3;
                UpdateStars(currentRating);
            };
            imgBtn4.Click += delegate
            {
                currentRating = 4;
                UpdateStars(currentRating);
            };
            imgBtn5.Click += delegate
            {
                currentRating = 5;
                UpdateStars(currentRating);
            };
            imgBtn6.Click += delegate
            {
                currentRating = 6;
                UpdateStars(currentRating);
            };

            // data is in, let's show the dialog box
            dispModal.Show();
        }

        void UpdateStars(int rating)
        {
            var resources = new List<int>
            {
                Resource.Drawable.emptystar,
                Resource.Drawable.pinkstar,
                Resource.Drawable.purplestar,
                Resource.Drawable.greenstar,
                Resource.Drawable.bluestar,
                Resource.Drawable.yellowstar,
                Resource.Drawable.orangestar
            };

            switch (rating)
            {
                case 0:
                    imgBtn1.SetImageResource(resources[0]);
                    imgBtn2.SetImageResource(resources[0]);
                    imgBtn3.SetImageResource(resources[0]);
                    imgBtn4.SetImageResource(resources[0]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 1:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[0]);
                    imgBtn3.SetImageResource(resources[0]);
                    imgBtn4.SetImageResource(resources[0]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 2:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[0]);
                    imgBtn4.SetImageResource(resources[0]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 3:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[3]);
                    imgBtn4.SetImageResource(resources[0]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 4:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[3]);
                    imgBtn4.SetImageResource(resources[4]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 5:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[3]);
                    imgBtn4.SetImageResource(resources[4]);
                    imgBtn5.SetImageResource(resources[5]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 6:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[3]);
                    imgBtn4.SetImageResource(resources[4]);
                    imgBtn5.SetImageResource(resources[5]);
                    imgBtn6.SetImageResource(resources[6]);
                    break;
            }
        }

        void SetStartStars()
        {
            var resources = new List<int>
            {
                Resource.Drawable.emptystar,
                Resource.Drawable.pinkstar,
                Resource.Drawable.purplestar,
                Resource.Drawable.greenstar,
                Resource.Drawable.bluestar,
                Resource.Drawable.yellowstar,
                Resource.Drawable.orangestar
            };

            switch (initialRating)
            {
                case 0:
                    imgBtn1.SetImageResource(resources[0]);
                    imgBtn2.SetImageResource(resources[0]);
                    imgBtn3.SetImageResource(resources[0]);
                    imgBtn4.SetImageResource(resources[0]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 1:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[0]);
                    imgBtn3.SetImageResource(resources[0]);
                    imgBtn4.SetImageResource(resources[0]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 2:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[0]);
                    imgBtn4.SetImageResource(resources[0]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 3:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[3]);
                    imgBtn4.SetImageResource(resources[0]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 4:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[3]);
                    imgBtn4.SetImageResource(resources[4]);
                    imgBtn5.SetImageResource(resources[0]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 5:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[3]);
                    imgBtn4.SetImageResource(resources[4]);
                    imgBtn5.SetImageResource(resources[5]);
                    imgBtn6.SetImageResource(resources[0]);
                    break;
                case 6:
                    imgBtn1.SetImageResource(resources[1]);
                    imgBtn2.SetImageResource(resources[2]);
                    imgBtn3.SetImageResource(resources[3]);
                    imgBtn4.SetImageResource(resources[4]);
                    imgBtn5.SetImageResource(resources[5]);
                    imgBtn6.SetImageResource(resources[6]);
                    break;
            }
        }
    }
}