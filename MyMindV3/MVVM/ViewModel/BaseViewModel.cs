using GalaSoft.MvvmLight;
using MvvmFramework.Models;
using System;
using System.Collections.Generic;
using System.IO;
using MvvmFramework.Helpers;
using PCLStorage;
using MvvmFramework.Enums;
using System.Threading.Tasks;

namespace MvvmFramework.ViewModel
{
    public class BaseViewModel : ViewModelBase
    {
        public BaseViewModel()
        {
            UpdateSessionExpirationTime();
        }

        public bool IsConnected { get; set; }

        Dictionary<string, string> errorTitles;
        public Dictionary<string, string> ErrorTitles
        {
            get { return errorTitles; }
            set { Set(() => ErrorTitles, ref errorTitles, value); }
        }

        Dictionary<string, string> errorMessages;
        public Dictionary<string, string> ErrorMessages
        {
            get { return errorMessages; }
            set { Set(() => ErrorMessages, ref errorMessages, value); }
        }

        Dictionary<string, string> messages;
        public Dictionary<string, string> Messages
        {
            get { return messages; }
            set { Set(() => Messages, ref messages, value); }
        }

        public string GetErrorTitle(string key)
        {
            if (ErrorTitles == null)
                return string.Empty;

            var retval = string.Empty;
            var errorDict = ErrorTitles;
            if (errorDict != null)
            {
                errorDict.TryGetValue(key, out retval);
            }

            return retval;
        }

        public string GetErrorMessage(string key)
        {
            if (ErrorMessages == null)
                return string.Empty;

            var retval = string.Empty;
            var errorDict = ErrorMessages;
            if (errorDict != null)
            {
                errorDict.TryGetValue(key, out retval);
            }

            return retval;
        }

        public string GetMessage(string key)
        {
            if (Messages == null)
                return string.Empty;

            var retval = string.Empty;
            var errorDict = Messages;
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

        static SystemUser _systemUser;
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

        static ClinicianUser _clinicianUser;
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
            SystemUser = null;
            ClinicianUser = null;
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
                Set(() => IsBusy, ref isBusy, value, true);
            }
        }

        bool isUser;
        public bool IsUser
        {
            get { return isUser; }
            set { Set(() => IsUser, ref isUser, value); }
        }

        string userImage;
        public string GetUserImage
        {
            get { return SystemUser.Guid; }
        }

        public string GetClinicianImage
        {
            get { return ClinicianUser.ClinicianGUID; }
        }

        string filename;
        public string Filename
        {
            get { return filename; }
            set { Set(() => Filename, ref filename, value); }
        }

        public Stream GetImage
        {
            get { return new FileIO().LoadFile(Filename).Result; }
        }

        public string GetCurrentFolder
        {
            get { return FileSystem.Current.LocalStorage.Path; }
        }

        public bool FileExists
        {
            get
            {
                if (string.IsNullOrEmpty(Filename))
                    return false;
                return new FileIO().FileExists(Filename);
            }
        }

        public bool GetIsClinician
        {
            get { return SystemUser?.IsAuthenticated == 3; }
        }

        public void SendTrackingInformation(ActionCodes actionCode, int resId = 0, string info = "null", string clientId = "null")
        {
            var paramList = new List<string>
            {
                "UserGUID",
                GetIsClinician ? ClinicianUser.ClinicianGUID : SystemUser.Guid,
                "AuthToken",
                SystemUser.APIToken,
                "AccountType",
                SystemUser.IsAuthenticated.ToString(),
                "ActionCode",
                actionCode.ToString(),
                "ResourceID",
                resId.ToString(),
                "SearchData",
                info,
                "ClientGUID",
                clientId
            };
            Task.Run(async () => await Send.SendData("LogPageAccess", paramList.ToArray()));
        }

        public void SendBrokenLink(int resId, ActionCodes code)
        {
            var paramList = new List<string>
            {
                "UserGUID", GetIsClinician ? ClinicianUser.ClinicianGUID : SystemUser.Guid,
                "AuthToken", SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(),
                "ResourceID", resId.ToString()
            };
            Task.Run(async () => await Send.SendData("ReportBrokenLink", paramList.ToArray()));
            SendTrackingInformation(code, resId);
        }
    }
}
