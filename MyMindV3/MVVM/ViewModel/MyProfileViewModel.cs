using GalaSoft.MvvmLight.Views;
using MvvmFramework.Helpers;
using MvvmFramework.Models;
using System.IO;

namespace MvvmFramework.ViewModel
{
    public class MyProfileViewModel :BaseViewModel
    {
        INavigationService navService;
        IRepository sqlConn;

        public MyProfileViewModel(INavigationService nav, IRepository repo)
        {
            navService = nav;
            sqlConn = repo;
        }

        public Stream GetProfileImage(string filename)
        {
            return new FileIO().LoadFile(filename).Result;
        }

        public void UpdateSystemUser()
        {
            sqlConn.SaveData(SystemUser);
        }
    }
}
