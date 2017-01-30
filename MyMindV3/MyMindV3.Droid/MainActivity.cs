
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using Xamarin.Forms;

namespace MyMindV3.Droid
{
    [Activity(ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        public static MainActivity Activity { get; private set; }

        public static ISharedPreferences Prefs { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            MainActivity.Activity = this;
            Prefs = GetSharedPreferences("MyMindV3", FileCreationMode.Private);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            App.ScreenSize = new Size(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density,
                Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            ImageCircleRenderer.Init();
            PullToRefreshViewRenderer.Init();
            LoadApplication(new App());
        }
    }
}

