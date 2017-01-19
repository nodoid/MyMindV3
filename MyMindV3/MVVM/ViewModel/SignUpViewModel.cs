using System;
using MvvmFramework.Models;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Command;
using MvvmFramework.Webservices;

namespace MvvmFramework.ViewModel
{
    public class SignUpViewModel : BaseViewModel
    {
        IRepository _database;
        INavigationService navService;
        IDialogService dialogService;
        string DateOfBirth;

        public SignUpViewModel(INavigationService navigation, IRepository repo, IDialogService dialog)
        {
            navService = navigation;
            _database = repo;
            dialogService = dialog;
            DateOfBirth = DateTime.Now.ToString("d");
        }

        RelayCommand signInCommand;
        public RelayCommand SignInCommand
        {
            get
            {
                return signInCommand ??
                    (
                        signInCommand = new RelayCommand(() => navService.GoBack())
                    );
            }
        }

        public string Name { get; set; }
        public string PreferredName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string PasswordRepeated { get; set; }
        public string PinCode { get; set; }

        RelayCommand createSubmitCommand;
        public RelayCommand CreateSubmitCommand
        {
            get
            {
                return createSubmitCommand ??
                    (
                        createSubmitCommand = new RelayCommand(async () =>
                        {
                            var Result = "";

                            if (HasValidInput)
                            {
                                var systemUser = new SystemUser
                                {
                                    Name = Name,
                                    PreferredName = PreferredName,
                                    DateOfBirth = DateOfBirth,
                                    Email = Email,
                                    Phone = Phone,
                                    Password = Password,
                                    PinCode = PinCode,
                                };
                                
                                var resu = new UsersWebservice().RegisterWeb(systemUser);

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
                                }

                                /* if the client is a member only - just show limited version */
                                if (SystemUser.IsAuthenticated == 1)
                                {
                                    navService.NavigateTo(ViewModelLocator.MyLimitedMindKey);
                                }
                                /* else display full app */
                                else if (SystemUser.IsAuthenticated > 1)
                                {
                                    navService.NavigateTo(ViewModelLocator.MyMindKey);
                                }
                            }
                            else
                            {
                                await dialogService.ShowMessage(GetErrorTitle("Gen_Error"), GetErrorMessage("Registration_ErrorMessage"));
                                navService.GoBack();
                            }
                        })
                        );
            }
        }

        bool _hasValidInput;
        public bool HasValidInput
        {
            get
            {
                var rv = false;
                if ((!String.IsNullOrEmpty(Name)) && (!String.IsNullOrEmpty(Email)) && (!String.IsNullOrEmpty(Password)) && (!String.IsNullOrEmpty(PasswordRepeated))
                     && (!String.IsNullOrEmpty(Phone)) && (!String.IsNullOrEmpty(PinCode)))
                {

                    var Result = "";

                    if ((Name.Length < 4) || (Email.Length < 4) || (Password.Length < 6) || (PasswordRepeated.Length < 4)
                        || (Phone.Length < 4) || (PinCode.Length < 4))
                    {
                        rv = false;
                    }
                    else
                    {
                        /* name and password length greater then 4 - check if passwords match */
                        if (Password != PasswordRepeated)
                        {
                            rv = false;
                        }
                    }

                    rv = true;
                }

                return rv;
            }
            set
            {
                if (value != _hasValidInput)
                {
                    Set(() => HasValidInput, ref _hasValidInput, value, true);
                }
            }
        }
    }
}
