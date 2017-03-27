using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;
using MvvmFramework.Interfaces;

namespace MvvmFramework.ViewModel
{
    public class MyHelpViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyHelpViewModel(INavigationService nav, IConnectivity con)
        {
            navService = nav;
            if (con.IsConnected)
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Help_Page_View : ActionCodes.User_Help_Page_View);
        }
    }
}
