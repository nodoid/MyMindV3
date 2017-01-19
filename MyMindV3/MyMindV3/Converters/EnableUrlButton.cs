using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyMindV3.Helpers
{
    public class EnableUrlButton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var para = (string)parameter;
            if (para.ToLowerInvariant().Contains("href"))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
