using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyMindV3
{
    public class CorrectColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;
            return b ? Color.Gray : Color.Blue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.Blue;
        }
    }
}

