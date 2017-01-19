using System;
using SQLite.Net.Attributes;
using GalaSoft.MvvmLight;

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
        string _userImage;
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
                    Set(() => Guid, ref _guid, value, true);
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
                    Set(() => Name, ref _name, value, true);
                }
            }
        }

        public string ClinicianGUID
        {
            get { return clinicianGUID; }
            set { Set(()=>ClinicianGUID, ref clinicianGUID, value, true); }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (value != _email)
                {
                    Set(()=>Email, ref _email, value, true);
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
                    Set(()=>Role, ref _role, value, true);
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
                    Set(() => FunFact, ref _funfact, value, true);
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
                    Set(() => Phone, ref _phone, value, true);
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
                    Set(() => HCPID, ref _hcpID, value, true);
                }
            }
        }

        public string UserImage
        {
            get
            {
                return _userImage;

                if (!string.IsNullOrEmpty(App.Self.UserSettings.LoadSetting<string>("ClinicianImage", SettingType.String)))
                {
                    _userImage = App.Self.UserSettings.LoadSetting<string>("ClinicianImage", SettingType.String);

                    if (App.Self.UserSettings.LoadSetting<string>("ClinicianImageDatestamp", SettingType.String).Equals(ProfilePictureUploadTimestamp.ToString("g")))
                        return _userImage;
                    else
                    {
                        GetData.GetImage(ClinicianGUID, false).ContinueWith((t) =>
                        {
                            if (t.IsCompleted)
                            {
                                _userImage = string.Format("{0}/{1}.jpg", App.Self.PicturesDirectory, ClinicianGUID);
                            }
                        }).ConfigureAwait(false);
                    }
                    return _userImage;
                }


                var origPath = DependencyService.Get<IContent>().PicturesDirectory();

                if (image.Contains("jpg"))
                    image = image.Substring(0, image.LastIndexOf('.'));

                if (string.IsNullOrEmpty(image))
                {
                    _userImage = string.Format("{0}/{1}.jpg", App.Self.PicturesDirectory, ClinicianGUID);
                    if (!DependencyService.Get<IContent>().FileExists(_userImage))
                    {
                        var otherDir = App.Self.UserSettings.LoadSetting<string>("ImageDirectory", SettingType.String);
                        _userImage = string.Format("{0}/{1}.jpg", otherDir, ClinicianGUID);
                        if (!DependencyService.Get<IContent>().FileExists(_userImage))
                        {
                            GetData.GetImage(ClinicianGUID, false).ContinueWith((t) =>
                    {
                        if (t.IsCompleted)
                        {
                            _userImage = string.Format("{0}/{1}.jpg", App.Self.PicturesDirectory, ClinicianGUID); ;
                        }
                    }).ConfigureAwait(false);
                        }
                        if (!DependencyService.Get<IContent>().FileExists(_userImage))
                            _userImage = "male_female.png";
                    }
                }

                return _userImage;
            }

            set
            {
                SettingType(() => UserImage, ref _userImage, value, true);
            }
        }

        public string APIToken
        {
            get { return apitoken; }
            set { Set(()=>APIToken, ref apitoken, value, true); }
        }

        public string PictureFilePath
        {
            get { return pictureFilePath; }
            set { Set(()=>PictureFilePath, ref pictureFilePath, value, true); }
        }

        public DateTime ProfilePictureUploadTimestamp
        {
            get { return profilePictureUploadDateStamp; }
            set { Set(()=>ProfilePictureUploadTimeStamp, ref profilePictureUploadDateStamp, value, true); }
        }

        public DateTime APITokenExpiry
        {
            get { return apiTokenExpiry; }
            set { Set(()=>APITokenExpiry, ref apiTokenExpiry, value, true); }
        }

        public bool IsClinician
        {
            get { return isClinician; }
            set { Set(()=>IsClinician, ref isClinician, value, true); }
        }
    }
}
