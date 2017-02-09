using GalaSoft.MvvmLight.Views;

namespace MvvmFramework.ViewModel
{
    public class MyHelpViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyHelpViewModel(INavigationService nav)
        {
            navService = nav;
        }
    }
}
