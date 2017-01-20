using GalaSoft.MvvmLight.Views;
using MvvmFramework.Helpers;
using MvvmFramework.Models;
using System.IO;

namespace MvvmFramework.ViewModel
{
    public class MyClinicianViewModel : BaseViewModel
    {
        INavigationService navService;
        IRepository sqlConn;
        public MyClinicianViewModel(INavigationService nav, IRepository repo)
        {
            navService = nav;
            sqlConn = repo;
        }

        public Stream GetClinicianImage(string filename)
        {
            return new FileIO().LoadFile(filename).Result; 
        }

        public void UpdateSystemUser()
        {
            sqlConn.SaveData<SystemUser>(SystemUser);
        }
    }
}
