using System;
using System.Collections.Generic;
using Foundation;
using MvvmFramework;
using MyMindV3.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserSettings))]
namespace MyMindV3.iOS
{
    public class UserSettings : IUserSettings
    {
        public void SaveSetting<T>(string name, T value, SettingType setting)
        {
            dynamic insertValue = null;
            switch (setting)
            {
                case SettingType.String:
                    insertValue = (NSString)(string)(object)value;
                    break;
                case SettingType.Bool:
                    insertValue = (NSNumber)(bool)(object)value;
                    break;
                case SettingType.Long:
                    insertValue = (NSNumber)(long)(object)value;
                    break;
                case SettingType.Float:
                    insertValue = (NSNumber)(float)(object)value;
                    break;
                case SettingType.Int:
                    insertValue = (NSNumber)(int)(object)value;
                    break;
            }

            NSUserDefaults.StandardUserDefaults[name] = insertValue;
        }

        public void SaveSetting(string name, List<string> vals)
        {
            // does nothing
        }

        public T LoadSetting<T>(string name, SettingType setting)
        {
            var value = NSUserDefaults.StandardUserDefaults[name];
            NSNumber val = null;

            dynamic returnValue = null;

            if (setting != SettingType.String)
                val = (NSNumber)value;

            if (val == null && setting != SettingType.String)
                return default(T);

            if (value == null)
                return default(T);

            switch (setting)
            {
                case SettingType.String:
                    returnValue = value.ToString();
                    break;
                case SettingType.Bool:
                    returnValue = val.BoolValue;
                    break;
                case SettingType.Float:
                    returnValue = val.FloatValue;
                    break;
                case SettingType.Int:
                    returnValue = val.Int32Value;
                    break;
                case SettingType.Long:
                    returnValue = val.LongValue;
                    break;
            }

            return (T)returnValue;
        }

        public List<string> LoadSetting(string name)
        {
            // does nothing
            return new List<string>();
        }
    }
}
