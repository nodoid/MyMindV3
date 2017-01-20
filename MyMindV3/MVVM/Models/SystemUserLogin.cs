using GalaSoft.MvvmLight;
using System;

namespace MvvmFramework.Models
{
    public class SystemUserLogin
    {
        private string _name;
        private string _password;
        private string _passwordRepeat;
        private string _result;
        private string _pincode;
        private DateTime _loginDateTime;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    //RaisePropertyChanged("Name");
                    //RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (value != _password)
                {
                    _password = value;
                    //RaisePropertyChanged("Password");
                    //RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string PasswordRepeat
        {
            get { return _password; }
            set
            {
                if (value != _passwordRepeat)
                {
                    _passwordRepeat = value;
                    //RaisePropertyChanged("PasswordRepeat");
                    //RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string Result
        {
            get { return _result; }
            set
            {
                if (value != _result)
                {
                    _result = value;
                    //RaisePropertyChanged("Result");
                }
            }
        }

        public string PinCode
        {
            get
            {

                //return _pincode;             
                if (_pincode == null)
                {
                    return "";
                }
                else
                {
                    return _pincode;
                }
            }
            set
            {
                if (value != _pincode)
                {
                    _pincode = value;
                    //RaisePropertyChanged("PinCode");
                    //OnPropertyChanged("HasValidInput");
                }
            }
        }

        public DateTime LoginDateTime
        {
            get { return _loginDateTime; }
            set
            {
                if (value != _loginDateTime)
                {
                    _loginDateTime = value;
                    //RaisePropertyChanged("LoginDateTime");
                }
            }
        }
    }
}
