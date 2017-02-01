using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyMindV3.Helpers
{
    public class ImageTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var prop = (string)parameter;
            var filename = string.Empty;
            var lc = prop.ToLowerInvariant();
            if (lc.Contains("jpg") || lc.Contains("jpeg"))
                filename = "jpg";
            if (lc.Contains("png"))
                filename = "png";
            if (lc.Contains("pdf"))
                filename = "pdf";
            if (lc.Contains("doc"))
                filename = "word";
            return filename;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
