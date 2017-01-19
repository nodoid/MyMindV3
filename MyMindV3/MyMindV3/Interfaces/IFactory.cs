using MyMindV3.Interfaces;
using NELFTCryptography;

namespace MyMindV3
{
    internal interface IFactory
    {
        IEncryptionManager GetEncryptionManager();

        ICryptographyService GetCryptographyService();
    }
}
