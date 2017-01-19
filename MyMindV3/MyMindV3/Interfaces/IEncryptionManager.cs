using MyMindV3.Classes;
using System.Collections.Generic;

namespace MyMindV3.Interfaces
{
    interface IEncryptionManager
    {
        IEnumerable<Appointment> DecryptAppointments(IEnumerable<Encryption> encryptions);

        bool IsEncryptionValid(Encryption encryption);
    }
}
