using GalaSoft.MvvmLight;
using MvvmFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvvmFramework.ViewModel
{
    public class BaseViewModel : ViewModelBase
    {
        public BaseViewModel()
        {
            UpdateSessionExpirationTime();
        }

        INetworkSpinner Spinner;
        public BaseViewModel(INetworkSpinner spinner)
        {
            Spinner = spinner;
        }

        bool isConnected;
        public bool IsConnected { get; set; }

        List<Dictionary<string, string>> errorTitles;
    public List<Dictionary<string, string>> ErrorTitles
        {
            get { return errorTitles; }
               set { Set(() => ErrorTitles, ref errorTitles, value); }
        }

        List<Dictionary<string, string>> errorMessages;
        public List<Dictionary<string, string>> ErrorMessages
        {
            get { return errorTitles; }
            set { Set(() => ErrorTitles, ref errorTitles, value); }
        }

        List<Dictionary<string, string>> messages;
        public List<Dictionary<string, string>> Messages
        {
            get { return messages; }
            set { Set(() => Messages, ref messages, value); }
        }

        public string GetErrorTitle(string key)
        {
            if (ErrorTitles == null)
                return string.Empty;

            var retval = string.Empty;
            var errorDict = ErrorTitles.FirstOrDefault(t => t.Keys.Contains(key));
            if (errorDict != null)
            {
                errorDict.TryGetValue(key, out retval);
            }

            return retval;
        }

        public string GetErrorMessage(string key)
        {
            if (Messages == null)
                return string.Empty;

            var retval = string.Empty;
            var errorDict = Messages.FirstOrDefault(t => t.Keys.Contains(key));
            if (errorDict != null)
            {
                errorDict.TryGetValue(key, out retval);
            }

            return retval;
        }

        public string GetMessage(string key)
        {
            if (ErrorMessages == null)
                return string.Empty;

            var retval = string.Empty;
            var errorDict = ErrorMessages.FirstOrDefault(t => t.Keys.Contains(key));
            if (errorDict != null)
            {
                errorDict.TryGetValue(key, out retval);
            }

            return retval;
        }

        string _errorOccurred;
        public string ErrorOccurred
        {
            get { return _errorOccurred; }
            set
            {
                if (value != _errorOccurred)
                {
                    Set(() => ErrorOccurred, ref _errorOccurred, value, true);
                }
            }
        }

SystemUser _systemUser;
        public SystemUser SystemUser
        {
            get { return _systemUser; }
            set
            {
                if (value != _systemUser)
                {
                    Set(() => SystemUser, ref _systemUser, value, true);
                }
            }
        }

ClinicianUser _clinicianUser;
        public ClinicianUser ClinicianUser
        {
            get { return _clinicianUser; }
            set
            {
                if (value != _clinicianUser)
                {
                    Set(() => ClinicianUser, ref _clinicianUser, value, true);
                }
            }
        }

DateTime _sessionExpirationTime;
        public DateTime SessionExpirationTime
        {
            get { return _sessionExpirationTime; }
            set
            {
                if (value != _sessionExpirationTime)
                {
                    _sessionExpirationTime = value;
                    RaisePropertyChanged("SessionExpirationTime");
                    RaisePropertyChanged("SessionExpirationTimeToString");
                }
            }
        }

        public string SessionExpirationTimeToString
        {
            get { return SessionExpirationTime.ToString("HH:mm:ss"); }
        }

        public void UpdateSessionExpirationTime()
        {
            var now = DateTime.Now;
            SessionExpirationTime = now.AddSeconds(10);
        }

        public void Logout()
        {
            _systemUser = null;
            _clinicianUser = null;
            SessionExpirationTime = System.DateTime.Now;
            SessionExpirationTime.AddHours(-5);
        }

        string spinnerTitle;
        public string SpinnerTitle
        {
            get { return spinnerTitle; }
            set { Set(() => SpinnerTitle, ref spinnerTitle, value); }
        }

        string spinnerMessage;
        public string SpinnerMessage
        {
            get { return spinnerMessage; }
            set { Set(() => SpinnerMessage, ref spinnerMessage, value); }
        }

        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                Spinner.NetworkSpinner(isBusy, SpinnerTitle, SpinnerMessage);
            }
        }
    }
}
