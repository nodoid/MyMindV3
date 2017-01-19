using System;
namespace MyMindV3
{
    public enum SettingType
    {
        Bool,
        Float,
        Int,
        Long,
        String,
        StringSet
    }

    public interface IUserSettings
    {
        void SaveSetting<T>(string name, T value, SettingType setting);

        T LoadSetting<T>(string name, SettingType setting);
    }
}
