using System;

using Xamarin.Forms;

namespace MyMindV3
{
    public class CustomChatView : View
    {
        public static readonly BindableProperty UsernameProperty = BindableProperty.Create(
        propertyName: "Username",
        returnType: typeof(string),
        declaringType: typeof(CustomChatView),
                    defaultValue: string.Empty);

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }
    }
}

