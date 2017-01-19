using MyMindV3.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMindV3.Interfaces
{
    public interface ICareMapService
    {
        Task<IEnumerable<Encryption>> GetPatientAppointments(string hcp);
    }
}
