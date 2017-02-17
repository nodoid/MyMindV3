using GalaSoft.MvvmLight.Views;
using MvvmFramework.Enums;

namespace MvvmFramework.ViewModel
{
    public class MyChatViewModel : BaseViewModel
    {
        public MyChatViewModel(INavigationService nav)
        {
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Client_Chat_Page_View : ActionCodes.User_My_Chat_Page_View);
        }
    }
}
