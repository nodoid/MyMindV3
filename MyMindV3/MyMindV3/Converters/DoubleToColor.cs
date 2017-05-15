using System;
using System.Globalization;
using Xamarin.Forms;
namespace MyMindV3
{
    public class DoubleToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (double)value;
            return (int)b == 0 ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
