using MvvmFramework.Interfaces;
using NELFTCryptography;

namespace MvvmFramework
{
    internal class Factory : IFactory
    {
        #region Singleton

        public static readonly IFactory Instance = new Factory();

        #endregion

        #region IFactory Members

        ICryptographyService IFactory.GetCryptographyService()
        {
            return new CryptographyService();
        }

        IEncryptionManager IFactory.GetEncryptionManager()
        {
            IEncryptionManager encryptionManager = new EncryptionManager(this);

            return encryptionManager;
        }

        #endregion
    }
}
