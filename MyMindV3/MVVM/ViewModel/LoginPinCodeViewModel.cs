using GalaSoft.MvvmLight.Views;

namespace MvvmFramework.ViewModel
{
    public class LoginPinCodeViewModel : BaseViewModel
    {
        INavigationService _navigation;
        string _pinCodeResult, _pin1, _pin2, _pin3, _pin4;

        public LoginPinCodeViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            PinCodeResult = GetMessage("LoginPincode_Login");
        }

        public string PinCodeResult
        {
            get { return _pinCodeResult; }
            set
            {
                if (value != _pinCodeResult)
                {
                    Set(() => PinCodeResult, ref _pinCodeResult, value, true);
                }
            }
        }

        public string Pin1
        {
            get { return _pin1; }
            set
            {
                if (value != _pin1)
                {
                    _pin1 = value;
                    RaisePropertyChanged("Pin1");
                    RaisePropertyChanged("HasValidPinCodeInput");
                }
            }
        }
        public string Pin2
        {
            get { return _pin2; }
            set
            {
                if (value != _pin2)
                {
                    _pin2 = value;
                    RaisePropertyChanged("Pin2");
                    RaisePropertyChanged("HasValidPinCodeInput");
                }
            }
        }
        public string Pin3
        {
            get { return _pin3; }
            set
            {
                if (value != _pin3)
                {
                    _pin3 = value;
                    RaisePropertyChanged("Pin3");
                    RaisePropertyChanged("HasValidPinCodeInput");
                }
            }
        }
        public string Pin4
        {
            get { return _pin4; }
            set
            {
                if (value != _pin4)
                {
                    _pin4 = value;
                    RaisePropertyChanged("Pin4");
                    RaisePropertyChanged("HasValidPinCodeInput");
                }
            }
        }

        public bool HasValidPinCodeInput
        {
            get
            {
                return (!string.IsNullOrEmpty(Pin1) && !string.IsNullOrEmpty(Pin2) && !string.IsNullOrEmpty(Pin3) && 
                    !string.IsNullOrEmpty(Pin4)) ? true : false;
            }
        }
    }
}
