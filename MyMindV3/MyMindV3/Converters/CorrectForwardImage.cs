using System;
using System.Globalization;
using Xamarin.Forms;
namespace MyMindV3
{
    public class CorrectForwardImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;
            return b ? "right" : "rightgrey";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
