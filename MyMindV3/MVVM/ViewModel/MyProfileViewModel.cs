using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
using MvvmFramework.Helpers;
using System.IO;

namespace MvvmFramework.ViewModel
{
    public class MyProfileViewModel : BaseViewModel
    {
        INavigationService navService;
        IRepository sqlConn;

        public MyProfileViewModel(INavigationService nav, IRepository repo)
        {
            navService = nav;
            sqlConn = repo;
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Profile_Page_View :
                (SystemUser.IsAuthenticated == 2 ? ActionCodes.User_Profile_Page_View : ActionCodes.Member_Profile_Page_View));
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
            if (!GetIsClinician)
                SendTrackingInformation(ActionCodes.User_Updated_Profile);
            sqlConn.SaveData(SystemUser);
        }
    }
}
