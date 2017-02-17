using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;

namespace MvvmFramework.ViewModel
{
    public class MyHelpViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyHelpViewModel(INavigationService nav)
        {
            navService = nav;
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Help_Page_View : ActionCodes.User_Help_Page_View);
        }
    }
}
