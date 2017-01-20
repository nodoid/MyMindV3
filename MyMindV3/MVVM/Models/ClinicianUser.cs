using System;
using SQLite.Net.Attributes;

namespace MvvmFramework.Models
{
    public class ClinicianUser
    {
        private string _id;
        private string _guid;
        private string _name;
        private string _email;
        private string _role;
        private string _funfact;
        private string _phone;
        private string _hcpID;
        string apitoken;
        DateTime profilePictureUploadDateStamp;
        DateTime apiTokenExpiry;
        string pictureFilePath;
        bool isClinician;
        string clinicianGUID;

        [PrimaryKey, AutoIncrement]
        public string ClinicianUserID
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
                    _guid = value;
                    //Set(() => Guid, ref _guid, value, true);
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
                    //Set(() => Name, ref _name, value, true);
                }
            }
        }

        public string ClinicianGUID
        {
            get { return clinicianGUID; }
            set { clinicianGUID = value; }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (value != _email)
                {
                    _email = value;
                    //Set(()=>Email, ref _email, value, true);
                }
            }
        }

        public string Role
        {
            get { return _role; }
            set
            {
                if (value != _role)
                {
                    _role = value;
                    //Set(()=>Role, ref _role, value, true);
                }
            }
        }

        public string FunFact
        {
            get { return _funfact; }
            set
            {
                if (value != _funfact)
                {
                    _funfact = value;
                    //Set(() => FunFact, ref _funfact, value, true);
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
                    //Set(() => Phone, ref _phone, value, true);
                }
            }
        }

        public string HCPID
        {
            get { return _hcpID; }
            set
            {
                if (value != _hcpID)
                {
                    _hcpID = value;
                    //Set(() => HCPID, ref _hcpID, value, true);
                }
            }
        }

        string userImage;
        public string UserImage
        {
            get { return userImage; }
            set { userImage = value; }
        }

        public string APIToken
        {
            get { return apitoken; }
            set { apitoken = value;  }
        }

        public string PictureFilePath
        {
            get { return pictureFilePath; }
            set { pictureFilePath = value; }
        }

        public DateTime ProfilePictureUploadTimestamp
        {
            get { return profilePictureUploadDateStamp; }
            set { profilePictureUploadDateStamp = value; }
        }

        public DateTime APITokenExpiry
        {
            get { return apiTokenExpiry; }
            set { apiTokenExpiry = value; }
        }

        public bool IsClinician
        {
            get { return isClinician; }
            set { isClinician = value; }
        }
    }
}
