using NELFTCryptography;

namespace MvvmFramework.Interfaces
{
    internal interface IFactory
    {
        IEncryptionManager GetEncryptionManager();

        ICryptographyService GetCryptographyService();
    }
}
