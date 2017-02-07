using System;
using Foundation;
using UIKit;

namespace XFormsRadioButton.iOS
{
    public static class StringExtensions
    {
        public static nfloat StringHeight(this string text, UIFont font, float width)
        {
            var nativeString = new NSString(text);

            var rect = nativeString.GetBoundingRect(
                new System.Drawing.SizeF(width, float.MaxValue),
                NSStringDrawingOptions.UsesLineFragmentOrigin,
                new UIStringAttributes() { Font = font },
                null);

            return rect.Height;
        }
    }
}

