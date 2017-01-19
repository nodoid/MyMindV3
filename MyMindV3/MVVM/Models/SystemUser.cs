using System;
using SQLite.Net.Attributes;
using GalaSoft.MvvmLight;

namespace MvvmFramework.Models
{
    public class SystemUser
    {
        private int _id;
        private string _guid;
        private string _name;
        private string _preferredName;
        private string _email;
        private string _dob;
        private string _contactNo;
        private string _referralReason;
        private string _likes;
        private string _dislikes;
        private string _goals;
        private string _phone;
        private string _rioid;

        private string _password;
        private string _passwordRepeated;
        private string _pincode;

        string icann;
        string apitoken;
        DateTime profilePictureUploadDateStamp;
        DateTime apiTokenExpiry;
        string pictureFilePath;

        private string _result;
        private int _isAuthenticated;
        private bool _isAdmin;
        private bool _isLogged = false;

        private bool _showProfileDetails = true;
        private bool _editProfileDetails = false;

        string _userImage;

        [PrimaryKey, AutoIncrement]
        public int UserID
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                }
            }
        }

        public string Guid
        {
            get { return _guid; }
            set
            {
                if (value != _guid)
                {
                    Set(() => Guid, ref _guid, value, true);
                }
            }
        }

        public string APIToken
        {
            get { return apitoken; }
            set
            {
                if (value != apitoken)
                {
                    Set(() => APIToken, ref apitoken, value, true);
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string PreferredName
        {
            get { return _preferredName; }
            set
            {
                if (value != _preferredName)
                {
                    _preferredName = value;
                    RaisePropertyChanged("PreferredName");
                    RaisePropertyChanged("HasValidInput");
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
                    RaisePropertyChanged("Password");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string PasswordRepeated
        {
            get { return _passwordRepeated; }
            set
            {
                if (value != _passwordRepeated)
                {
                    _passwordRepeated = value;
                    RaisePropertyChanged("PasswordRepeated");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string PinCode
        {
            get { return _pincode; }
            set
            {
                if (value != _pincode)
                {
                    Set(() => PinCode, ref _pincode, value, true));
                }
            }
        }

        public string DateOfBirth
        {
            get { return _dob; }
            set
            {
                if (value != _dob)
                {
                    _dob = value.Split(' ')[0];
                    RaisePropertyChanged("DateOfBirth");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string DateOfBirthToString
        {
            get { return _dob; }
        }

        public string ContactNumber
        {
            get { return _contactNo; }
            set
            {
                if (value != _contactNo)
                {
                    _contactNo = value;
                    RaisePropertyChanged("ContactNumber");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string ReferralReason
        {
            get { return _referralReason; }
            set
            {
                if (value != _referralReason)
                {
                    _referralReason = value;
                    RaisePropertyChanged("ReferralReason");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string Likes
        {
            get { return _likes; }
            set
            {
                if (value != _likes)
                {
                    _likes = value;
                    RaisePropertyChanged("Likes");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string Dislikes
        {
            get { return _dislikes; }
            set
            {
                if (value != _dislikes)
                {
                    _dislikes = value;
                    RaisePropertyChanged("Dislikes");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string Goals
        {
            get { return _goals; }
            set
            {
                if (value != _goals)
                {
                    _goals = value;
                    RaisePropertyChanged("Goals");
                    RaisePropertyChanged("HasValidInput");
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
                    Set(() => Result, ref _result, value, true);
                }
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                if (value != _phone)
                {
                    _phone = value;
                    RaisePropertyChanged("Phone");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (value != _email)
                {
                    _email = value;
                    RaisePropertyChanged("Email");
                    RaisePropertyChanged("HasValidInput");
                }
            }
        }

        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                if (value != _isAdmin)
                {
                    Set(() => IsAdmin, ref _isAdmin, value, true);
                }
            }
        }

        public int IsAuthenticated
        {
            get { return _isAuthenticated; }
            set
            {
                if (value != _isAuthenticated)
                {
                    Set(() => IsAuthenticated, ref _isAuthenticated, value, true);
                }
            }
        }

        public bool IsLogged
        {
            get { return _isLogged; }
            set
            {
                if (value != _isLogged)
                {
                    Set(() => IsLogged, ref _isLogged, value, true);
                }
            }
        }

        public string RIOID
        {
            get { return _rioid; }
            set
            {
                if (value != _rioid)
                {
                    Set(() => RIOID, ref _rioid, value, true);
                }
            }
        }

        public string ICANN
        {
            get { return icann; }
            set
            {
                if (value != icann)
                {
                    Set(() => ICANN, ref icann, value, true);
                }
            }
        }

        public string PictureFilePath
        {
            get { return pictureFilePath; }
            set { Set(() => PictureFilePath, ref pictureFilePath, value, true); }
        }

        public DateTime ProfilePictureUploadTimestamp
        {
            get { return profilePictureUploadDateStamp; }
            set { Set(() => ProfilePictureUploadTimestamp, ref profilePictureUploadDateStamp, value,true); }
        }

        public DateTime APITokenExpiry
        {
            get { return apiTokenExpiry; }
            set { Set(() => APITokenExpiry, ref apiTokenExpiry, value, true); }
        }

        public string UserImage
        {
            get
            {
                return _userImage;
            }
            set
            {
                Set(() => UserImage, ref _userImage, value, true);
            }
        }

        /* control which section is displayed */
        public bool ShowProfileDetails
        {
            get { return _showProfileDetails; }
            set
            {
                if (value != _showProfileDetails)
                {
                    Set(() => ShowProfileDetails, ref _showProfileDetails, value, true);
                }
            }
        }

        public bool EditProfileDetails
        {
            get { return _editProfileDetails; }
            set
            {
                if (value != _editProfileDetails)
                {
                    Set(() => EditProfileDetails, ref _editProfileDetails, value, true);
                }
            }
        }
    }
}
