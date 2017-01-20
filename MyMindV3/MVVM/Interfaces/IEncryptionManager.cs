using MvvmFramework.Models;
using System.Collections.Generic;

namespace MvvmFramework.Interfaces
{
    interface IEncryptionManager
    {
        IEnumerable<Appointment> DecryptAppointments(IEnumerable<Encryption> encryptions);

        bool IsEncryptionValid(Encryption encryption);
    }
}
