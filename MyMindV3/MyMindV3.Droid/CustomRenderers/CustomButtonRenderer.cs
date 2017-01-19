using Android.Graphics.Drawables;
using MyMindV3;
using MyMindV3.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomButton), typeof(CustomButtonRenderer))]
namespace MyMindV3.Droid
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        GradientDrawable _normal, _pressed;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.ButtonTop));
            }
        }

        /*protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var button = (Xamarin.Forms.Button)sender;

            if (_normal != null && _pressed != null)
            {
                if (e.PropertyName == "BorderRadius")
                {
                    _normal.SetCornerRadius(button.BorderRadius);
                    _pressed.SetCornerRadius(button.BorderRadius);
                }
                if (e.PropertyName == "BorderWidth" || e.PropertyName == "BorderColor")
                {
                    _normal.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
                    _pressed.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
                }
            }
        }*/
    }
}
