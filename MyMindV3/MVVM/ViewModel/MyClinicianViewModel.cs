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

        string filename;
        public string ImageFilename
        {
            get { return filename; }
            set { Set(() => ImageFilename, ref filename, value); }
        }

        public Stream GetProfileImage
        {
            get
            {
                GetData.GetImage(ImageFilename, IsUser).ConfigureAwait(true);
                return new FileIO().LoadFile(ImageFilename).Result;
            }
        }

        public void UpdateSystemUser()
        {
            sqlConn.SaveData<SystemUser>(SystemUser);
        }
    }
}
