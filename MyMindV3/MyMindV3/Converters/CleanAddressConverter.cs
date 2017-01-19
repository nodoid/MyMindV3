using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyMindV3.Helpers
{
    public class CleanAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string)parameter;
            var rettext = string.Empty;
            var idx = text.IndexOf('>');
            if (idx == -1)
                return text;
            else
            {
                var part = text.Substring(idx, text.Length - idx);
                var idxend = part.IndexOf('<');
                rettext = part.Substring(0, idxend);
            }
            return rettext;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)parameter;
        }
    }
}
