using Xamarin.Forms;

namespace MyMindV3
{
    public class CustomImage : Image
    {
        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create("ImageSource", typeof(string), typeof(CustomImage), default(string), propertyChanged: (bindable, oldValue, newValue) =>
                {
                    if (Device.OS != TargetPlatform.Android)
                    {
                        var image = (CustomImage)bindable;

                        var baseImage = (Image)bindable;
                        baseImage.Source = image.ImageSource;
                    }
                });

        public string ImageSource
        {
            get { return GetValue(ImageSourceProperty) as string; }
            set
            {
                SetValue(ImageSourceProperty, value);
                OnPropertyChanged("ImageSource");
            }
        }
    }
}
