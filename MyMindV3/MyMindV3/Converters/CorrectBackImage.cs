using System;
using System.Globalization;
using Xamarin.Forms;
namespace MyMindV3
{
    public class CorrectBackImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;
            return b ? "left" : "leftgrey";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
