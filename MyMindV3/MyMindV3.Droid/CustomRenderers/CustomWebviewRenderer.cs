using Xamarin.Forms.Platform.Android;
using MyMindV3;
using Xamarin.Forms;
using MyMindV3.Droid;
using Android.App;
using Android.Views;

[assembly: ExportRenderer(typeof(CustomWebview), typeof(CustomWebviewRenderer))]
namespace MyMindV3.Droid
{
    public class CustomWebviewRenderer : WebViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null) return;

            Control.FocusChange += Control_FocusChange;
        }

        void Control_FocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustResize);
            else
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustNothing);
        }

    }
}
