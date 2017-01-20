using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace MvvmFramework.ViewModel
{
    public class MyMindViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyMindViewModel(INavigationService nav)
        {
            navService = nav;
        }

        RelayCommand logoutCommand;
        public RelayCommand LogoutCommand
        {
            get
            {
                return logoutCommand ?? (new RelayCommand(() => navService.NavigateTo(ViewModelLocator.LoginKey)));
            }
        }

        RelayCommand showMyProfileCommand;
        public RelayCommand ShowMyProfileCommand
        {
            get
            {
                return showMyProfileCommand ?? (new RelayCommand(() => navService.NavigateTo(ViewModelLocator.MyProfileKey)));
            }
        }

        RelayCommand showMyPatientCommand;
        public RelayCommand ShowMyPatientCommand
        {
            get
            {
                return showMyPatientCommand ?? (new RelayCommand(() => navService.NavigateTo(ViewModelLocator.MyPatientKey)));
            }
        }

        RelayCommand showMyClinicianCommand;
        public RelayCommand ShowMyClinicianCommand
        {
            get
            {
                return showMyClinicianCommand ?? (new RelayCommand(() => navService.NavigateTo(ViewModelLocator.MyClinicianKey)));
            }
        }

        RelayCommand showMyJourneyCommand;
        public RelayCommand ShowMyJourneyCommand
        {
            get
            {
                return showMyJourneyCommand ?? (new RelayCommand(() => navService.NavigateTo(ViewModelLocator.MyJourneyKey)));
            }
        }

        RelayCommand showMyChatCommand;
        public RelayCommand ShowMyChatCommand
        {
            get
            {
                return showMyChatCommand ?? (new RelayCommand(() =>
                {
                    var guid = SystemUser.IsAuthenticated == 3 ? ClinicianUser.ClinicianGUID : SystemUser.Guid;
                    navService.NavigateTo(ViewModelLocator.MyChatKey, guid);
                }));
            }
        }

        RelayCommand showMyPlansCommand;
        public RelayCommand ShowMyPlansCommand
        {
            get
            {
                return showMyPlansCommand ?? (new RelayCommand(() => navService.NavigateTo(ViewModelLocator.MyPlansKey)));
            }
        }

        RelayCommand showMyResourcesCommand;
        public RelayCommand ShowMyResourcesCommand
        {
            get
            {
                return showMyResourcesCommand ?? (new RelayCommand(() => navService.NavigateTo(ViewModelLocator.MyResourcesKey)));
            }
        }
    }
}
