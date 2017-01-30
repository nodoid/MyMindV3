using System.Threading;
using Android.App;
using Android.OS;

namespace MyMindV3.Droid
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, Icon = "@drawable/icon")]
    public class Splash : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Thread.Sleep(500);
            StartActivity(typeof(MainActivity));
        }
    }
}

