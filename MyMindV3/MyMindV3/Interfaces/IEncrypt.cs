using System;
namespace MyMindV3
{
    public interface IEncrypt
    {
        string Iv_To_Pass_To_Encryption { get; }

        string EncryptString(string text, string key);
    }
}
