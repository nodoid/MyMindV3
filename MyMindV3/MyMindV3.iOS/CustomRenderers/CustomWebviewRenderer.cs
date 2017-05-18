using System;
using System.Threading.Tasks;
using MyMindV3;
using MyMindV3.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomWebview), typeof(WebViewRender))]
namespace MyMindV3.iOS
{
    public class WebViewRender : WebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            var webView = e.NewElement as CustomWebview;
            if (webView != null)
                webView.EvaluateJavascript = (js) =>
                {
                    return Task.FromResult(this.EvaluateJavascript(js));
                };
        }
    }
}
