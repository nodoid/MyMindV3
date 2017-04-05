using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MvvmFramework.Interfaces;

namespace MvvmFramework.ViewModel
{
    public class MyLimitedMindViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyLimitedMindViewModel(INavigationService nav, IConnectivity con)
        {
            navService = nav;
            if (con.IsConnected)
                SendTrackingInformation(Enums.ActionCodes.Member_Login);
        }

        RelayCommand logoutCommand;
        public RelayCommand LogoutCommand
        {
            get
            {
                return logoutCommand ?? (logoutCommand = new RelayCommand(() =>
                {
                    DisplayLogin = true;
                    DisplayAcceptance = false;
                    navService.NavigateTo(ViewModelLocator.LoginKey);
                }));
            }
        }

        RelayCommand showMyProfileCommand;
        public RelayCommand ShowMyProfileCommand
        {
            get
            {
                return showMyProfileCommand ?? (showMyProfileCommand = new RelayCommand(() => navService.NavigateTo(ViewModelLocator.MyProfileKey)));
            }
        }

        RelayCommand showMyResourcesCommand;
        public RelayCommand ShowMyResourcesCommand
        {
            get
            {
                return showMyResourcesCommand ?? (showMyResourcesCommand = new RelayCommand(() => navService.NavigateTo(ViewModelLocator.MyResourcesKey)));
            }
        }
    }
}
