using MvvmFramework.Interfaces;
using MvvmFramework.Models;
using NELFTCryptography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmFramework.Helpers
{
    internal class EncryptionManager : IEncryptionManager
    {
        private const string PrivateKey = "<RSAKeyValue><Modulus>y4+1v2NcgvgB7ObZc68JOHFD7zGygjZYgyWjB4aqRivklLYOXxc6zkNV9+CH1TxGe0zwdfZkxvtO2TSRNbQjma32tD3/UpptEbjG/M5ZxRAmwc+fOWlNNr/qFS+oryKjbRklfOzpjb31bECR6xg444qBKkuHFfrZ+Pj8gR8C2treZ1j9xM/D4yYiyB7hs0X1xuSCCvU31y5pgiu2l84+jSN3peghE+n2XMjgpFG58QbjAsMTk6IV9ptxtrNERKoRHyEdUzyHd8antiyEaP8ZpBKOp1SlCLcOdLz5BybGWomAi4q/TswAZ85DQB4a8gaeKrwhsIahQElsc2PizQ5CSw==</Modulus><Exponent>AQAB</Exponent><P>5GrvuVupmn+o0I7ADJkUBjwsQBxnlsnirEEmjlkblEJHaoqbZutYJTSMIXThbBuPgvpj2Lr/CGB58EfLw3AmLbGV4UoQPGeaN+lErlwgTr++a1hW4lrHVvr6M/60al6LMPapGx+ajOSHOoi74oWaOsNcIJFCNxiAwrHqiBxdVSE=</P><Q>5CRj+jHmGwGCiD5A9BwNn+uErfDdFS86RAPhWrcv+1F6fAXNk7kj8A/DivkN282IQva1mqlE9GvuK8gymRY2qNKQAabB1ip/rds8ByfzHaShAeA6o09I5JS2IuR/9/l4Yb718DPRloQs58bXXygkji8plcdNVwfq7hmnm0bQfes=</Q><DP>aQ7iSa/mN0BmJd6yaFj0A9YjSC2IW6tpjW69zBUiATPm4xLzXQf718judujqUa3veP+vcty3NmQhUanEB5UKilO2qxNEmnPQU8z/oO6QkYfRjk/oTl4Qd2oiME4DLzVi8ddRej8z93+Yzwxa1lo60LJQKk6FVQ/5EROB2q6pIuE=</DP><DQ>BhamiGXOg0f+CM7IgvtArdTmPJIiBE+3WVtVJ0uePzHEeAzbmKanO9/f0xSkdwo8KbVV0WPVEETVHVwBZa5/FPBRoYmmwKVvtN8V8gbx6QecuGUYi4UxJ9CEL9gzRhXlTY6AF8H6RO8QF7tpwgNbPmQp0vJ3sOvoLCSJIY65P7M=</DQ><InverseQ>dYyewgQF+eQ0W4jfJe3KhZEPrlCZsn1M0580YCy9yAoeFVm5pBtehvq5TVAw+D9fIlD6IWFAjlarz8ilPsE1AIlyaGZ3Hy3+1LO1N8sEG5ypNPmuXb/j3gGRFIvVkbjT7/srhCSqRihgcfB0Lilj7hZvNBd5OMOGZrd3Gb5DHj4=</InverseQ><D>c3oRBoOdZYa/wN6smR3Aw04t1bGBrLCbwbpGI+zXB3u67knhGTyaVYyJZlbvd5379dKNzr16Kg9CiaFyht6sG3A+tb097GRCX31NFPkSn+IrumLH1s/Px7FTX7Vrof6qayeKxrW3QHt0kHaI0K8rBpn58D5sYaGuqbjFtZIcEn54D9XgZjtmbVrR3izzB/kdWhjuaUoP7mhHONHotGkETeB1Z5Z7/VZ+DV4kqn1qZunUucJaZsSgzfnVKv8RQDmbd3PxiQcPTYOVej2W3vJ7k9lQq/7w+sb0eI+UI4RMC8piQ6q0rXOScP0nlo2N91gwsvTcLEizhq5I2Ej6p1t0gQ==</D></RSAKeyValue>";
        private const string ServerPublicKey = "<RSAKeyValue><Modulus>ydFbx9X47soMOjozCs3xV/Jvkvh1AnRivNAAvbhsPoQsfrQ4qV1H/F1x+7+d4UNh91xuB3PpFfvEu4uaVRrQilGaQbSZFjwVbjNcYPF+Z2cHpP8J6j3TsHlEk2b56FiqgQs7z9b5FXlI4OgDOdE4EOFheOBdFFejb5AthhQvg80HDdn/7cHGOPA44uJRG/2MJtnkHXU3w4UhWPkJKiYGcmjBQzPFUOlVv5RK4gfG1Sje+t0nTs8RCwfTuKe3Lzh9WHPbgXS0BSf1RotNUYurF+PDygt6uj38DOCasDbxn04T5qLKdFn0XB1y6FbPeYoSWOmsW6le/SIjUDEtLxbh7w==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private readonly ICryptographyService _cryptographyService;
        private readonly IFactory _factory;

        public EncryptionManager(IFactory factory)
        {
            _factory = factory;
            _cryptographyService = factory.GetCryptographyService();
        }

        bool IEncryptionManager.IsEncryptionValid(Encryption encryption)
        {
            var dataToDecrypt = Convert.FromBase64String(encryption.Value);
            var rsaEncryptedkey = Convert.FromBase64String(encryption.Key);
            var iv = Convert.FromBase64String(encryption.IV);

            var hashmac = Convert.FromBase64String(encryption.Hashmac);

            var aesKey = _cryptographyService.RSADecrypt(rsaEncryptedkey, PrivateKey);

            var hashmacToMatch = _cryptographyService.ComputeHashSha256(dataToDecrypt, aesKey);

            var isHashmac = CompareArrays(hashmac, hashmacToMatch);

            return isHashmac;

        }


        IEnumerable<Appointment> IEncryptionManager.DecryptAppointments(IEnumerable<Encryption> encryptions)
        {
            IList<Appointment> appointments = new List<Appointment>();

            foreach (var encryption in encryptions)
            {
                var dataToDecrypt = Convert.FromBase64String(encryption.Value);
                var rsaEncryptedkey = Convert.FromBase64String(encryption.Key);
                var iv = Convert.FromBase64String(encryption.IV);

                var aesKey = _cryptographyService.RSADecrypt(rsaEncryptedkey, PrivateKey);

                var decryptedData = _cryptographyService.AesDecrypt(dataToDecrypt, aesKey, iv);

                var decryptedString = Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);

                var appointmentObjects = JsonConvert.DeserializeObject<IEnumerable<Appointment>>(decryptedString);

                foreach (var app in appointmentObjects)
                {
                    appointments.Add(app);
                }
            }

            return appointments;
        }

        private bool CompareArrays(byte[] array1, byte[] array2)
        {
            var result = array1.Length == array2.Length;

            for (var i = 0; i < array1.Length && i < array2.Length; i++)
            {
                result &= array1[i] == array2[i];
            }

            return result;
        }
    }
}
