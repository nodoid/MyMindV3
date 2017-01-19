using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MvvmFramework.Models;
using MvvmFramework.Webservices;

namespace MvvmFramework.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        INavigationService navService;
        IDialogService dialogService;
        IRepository repo;

        public LoginViewModel(INavigationService navigation, IDialogService dialog, IRepository sql)
        {
            navService = navigation;
            dialogService = dialog;
            repo = sql;
            DisplayLogin = true;
            DisplayAcceptance = false;
            HasValidInput = false;
        }

        RelayCommand loginCommand;
        public RelayCommand LoginCommand
        {
            get
            {
                return loginCommand ??
                    (
                    loginCommand = new RelayCommand(() =>
                    {
                        var Result = string.Empty;
                        if (HasValidInput)
                {
                    DisplayLogin = false;
                    DisplayAcceptance = true;
                }
                else
                {
                    Result = "There was a problem with your username or password - please recheck";
                }
                    })
                );
            }
        }

        RelayCommand acceptCommand;
        public RelayCommand AcceptCommand
        {
            get
            {
                return acceptCommand ??
                    (
                        acceptCommand = new RelayCommand(async () =>
                        {
                            if (IsConnected)
                            {
                                SpinnerTitle = GetMessage("LoggingIn_Title");
                                SpinnerMessage = GetMessage("Gen_PleaseWait");
                                IsBusy = true;
                                if (ProcessLogin())
                                {
                                    IsBusy = false;
                                    if (SystemUser.IsAuthenticated < 1)
                                    {
                                        IsBusy = false;
                                        await dialogService.ShowMessage(GetErrorTitle("Gen_Error"), GetErrorMessage("Login_InvalidError"));
                                    }
                                    /* if the client is a member only - just show limited version */
                                    if (SystemUser.IsAuthenticated == 1)
                                    {
                                        navService.NavigateTo(ViewModelLocator.MyLimitedMindView);
                                    }
                                    /* else display full app */
                                    else if (SystemUser.IsAuthenticated > 1)
                                    {
                                        navService.NavigateTo(ViewModelLocator.MyMindView);
                                    }
                                }
                                else
                                {
                                    SpinnerMessage = SpinnerTitle = string.Empty;
                                    IsBusy = false;
                                    await dialogService.ShowMessage(GetErrorTitle("Gen_Error"), GetErrorMessage("Login_InvalidError"));
                                }
                            }
                        })
                        );
            }
        }

        RelayCommand signupCommand;
        public RelayCommand SignupCommand
        {
            get
            {
                return signupCommand ??
                    (
                    signupCommand = new RelayCommand(async () =>
                    {
                        ResetInputs();
                        await navService.NavigateTo(ViewModelLocator.SignupView);
                    })
                    );
            }
        }

        public string Name { get; set; }
        public string Password { get; set; }

        public bool ProcessLogin()
        {
            var resu = new UsersWebservice().LoginUser(Name, Password);

            if (resu != null)
            {
                SystemUser = new SystemUser
                {
                    ContactNumber = resu.ContactNumber,
                    DateOfBirth = resu.DateOfBirth,
                    Dislikes = resu.Dislikes,
                    Goals = resu.Goals,
                    Guid = resu.UserGUID,
                    IsAuthenticated = resu.IsAuthenticated,
                    IsLogged = true,
                    Likes = resu.Likes,
                    Name = resu.Name,
                    Phone = resu.ContactNumber,
                    PinCode = resu.Pincode,
                    PreferredName = resu.Name,
                    ReferralReason = resu.ReferralReason,
                    RIOID = resu.RIOID,
                    UserImage = string.Empty,
                    ICANN = resu.RIOID,
                    APIToken = resu.APIToken,
                    PictureFilePath = resu.PictureFilePath,
                    APITokenExpiry = resu.APITokenExpiry
                };

                if (resu.AssignedClinician != null)
                {
                    ClinicianUser = new ClinicianUser
                    {
                        ClinicianGUID = resu.AssignedClinician.ClinicianGUID,
                        Email = resu.AssignedClinician.Email,
                        FunFact = resu.AssignedClinician.FunFact,
                        Guid = resu.AssignedClinician.ClinicianGUID,
                        Name = resu.AssignedClinician.Name,
                        Phone = resu.AssignedClinician.ContactNumber,
                        Role = resu.AssignedClinician.WhatIDo,
                        HCPID = resu.AssignedClinician.HCPID,
                        UserImage = string.Empty,
                        APIToken = resu.APIToken,
                        PictureFilePath = !string.IsNullOrEmpty(resu.PictureFilePath) ? resu.PictureFilePath : string.Empty
                    };
                    ClinicianUser.APIToken = resu.APIToken;
                }
                else
                {
                    ClinicianUser = new ClinicianUser
                    {
                        HCPID = resu.HCPID,
                        Email = resu.Email,
                        FunFact = resu.FunFact,
                        ClinicianGUID = resu.ClinicianGUID,
                        Name = resu.Name,
                        Phone = resu.ContactNumber,
                        Role = resu.WhatIDo,
                        UserImage = string.Empty,
                        APIToken = resu.APIToken,
                        APITokenExpiry = resu.APITokenExpiry,
                    };
                }

                return true;
            }
            
            ResetInputs();
            return false;
        }

        public void ResetInputs()
        {
            Name = null;
            Password = null;
            DisplayLogin = true;
            DisplayAcceptance = false;
        }

        private bool _hasValidInput;
        public bool HasValidInput
        {
            get
            {
                if ((!string.IsNullOrEmpty(Name)) && (!string.IsNullOrEmpty(Password)))
                {
                    return (Name.Length < 4) || (Password.Length < 6) ? false : true;
                }

                return false;
            }
            set
            {
                if (value != _hasValidInput)
                {
                    Set(() => HasValidInput, ref _hasValidInput, value, true);
                }
            }
        }


        private bool _checkBoxImg;
        public bool CheckBoxImg
        {
            get { return _checkBoxImg; }
            set
            {
                if (value != _checkBoxImg)
                {
                    Set(() => CheckBoxImg, ref _checkBoxImg, value, true);
                }
            }
        }

        private string _checkImg;
        public string CheckImg
        {
            get { return _checkImg; }
            set
            {
                _checkImg = CheckBoxImg ? "check.png" : "empty_check.png";
            }
        }

        bool _displayLogin;
        public bool DisplayLogin
        {
            get { return _displayLogin; }
            set
            {
                if (value != _displayLogin)
                {
                    Set(() => DisplayLogin, ref _displayLogin, value, true);
                }
            }
        }

        bool _displayAcceptance;
        public bool DisplayAcceptance
        {
            get { return _displayAcceptance; }
            set
            {
                if (value != _displayAcceptance)
                {
                    Set(() => DisplayAcceptance, ref _displayAcceptance, value, true);
                }
            }
        }
    }
}
