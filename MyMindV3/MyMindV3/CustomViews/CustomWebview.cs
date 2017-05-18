using System;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace MyMindV3
{
    public class CustomWebview : WebView
    {
        /*public CustomWebview CreateCustomWebview(AbsoluteLayout layout)
        {
            AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.All);

            layout.Children.Add(this);

            return this;
        }

        public void UpdatePosition(BindableObject bindableObject, double x, double y, double width, double height)
        {
            AbsoluteLayout.SetLayoutBounds(bindableObject, new Rectangle(x, y, width, height));
            AbsoluteLayout.SetLayoutFlags(bindableObject, AbsoluteLayoutFlags.None);
        }*/

        public static BindableProperty EvaluateJavascriptProperty =
            BindableProperty.Create(nameof(EvaluateJavascript), typeof(Func<string, Task<string>>), typeof(CustomWebview), null, BindingMode.OneWayToSource);

        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return (Func<string, Task<string>>)GetValue(EvaluateJavascriptProperty); }
            set { SetValue(EvaluateJavascriptProperty, value); }
        }
    }
}
