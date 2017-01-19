using GalaSoft.MvvmLight;

namespace MvvmFramework.ViewModel
{
    public class BaseViewModel : ViewModelBase
    {
        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { Set(() => IsBusy, ref isBusy, value, true); }
        }
    }
}
