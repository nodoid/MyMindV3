using MyMindV3.Droid;
using Xamarin.Forms;
using Android.App;

[assembly: Dependency(typeof(NetworkSpin))]
namespace MyMindV3.Droid
{
    public class NetworkSpin : INetworkSpinner
    {
        static ProgressDialog progress;

        public void NetworkSpinner(bool on, string title, string message)
        {
            if (on)
            {
                MainActivity.Activity.RunOnUiThread(delegate
                {
                    progress = ProgressDialog.Show(Forms.Context, title, message);
                });
            }
            else
            {
                MainActivity.Activity.RunOnUiThread(delegate
                {
                    progress.Dismiss();
                });
            }
        }
    }
}

