using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
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
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Profile_Page_View : ActionCodes.User_My_Clinician_Page_View);
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
            SendTrackingInformation(ActionCodes.Clinician_Updated_Profile);
            sqlConn.SaveData<SystemUser>(SystemUser);
        }
    }
}
