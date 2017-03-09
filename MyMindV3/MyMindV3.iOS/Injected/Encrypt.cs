using System;
using MyMindV3.iOS;
using Xamarin.Forms;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using MvvmFramework;

[assembly: Dependency(typeof(Encrypt))]
namespace MyMindV3.iOS
{
    public class Encrypt : IEncrypt
    {
        public string Iv_To_Pass_To_Encryption { get; set; }

        const int keysize = 256;

        public string EncryptHcpString(string plainText)
        {
            var initVectorBytes = Encoding.UTF8.GetBytes(Constants.HcpInitVector);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var password = new PasswordDeriveBytes(Constants.HcpPassPhrase, null);
            var keyBytes = password.GetBytes(keysize / 8);
            var symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };
            byte[] cipherTextBytes;

            var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                }
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public string EncryptString(string text, string key)
        {
            byte[] encData;
            var aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 256,
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC
            };
            aes.GenerateIV();
            Iv_To_Pass_To_Encryption = Convert.ToBase64String(aes.IV);
            var encryptor = aes.CreateEncryptor(Encoding.Default.GetBytes(key), aes.IV);
            var textBytes = Encoding.Default.GetBytes(text);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(textBytes, 0, textBytes.Length);
                    cs.FlushFinalBlock();
                }
                encData = ms.ToArray();
            }

            return Convert.ToBase64String(encData);
        }
    }
}