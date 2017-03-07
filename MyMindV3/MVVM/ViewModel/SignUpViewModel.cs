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
        public string PostCode { get; set; }

        RelayCommand createSubmitCommand;
        public RelayCommand CreateSubmitCommand
        {
            get
            {
                return createSubmitCommand ??
                    (
                        createSubmitCommand = new RelayCommand(async () =>
                        {
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
                                    PostCode = PostCode
                                };

                                var resu = new UsersWebservice().RegisterWeb(systemUser);
                                if (resu != null)
                                {
                                    await dialogService.ShowMessage(GetMessage("RegUser_Completed_Message"), GetMessage("RegUser_Completed"));
                                }
                            }
                            else
                            {
                                await dialogService.ShowMessage(GetErrorMessage("Registration_ErrorMessage"), GetErrorTitle("Gen_Error"));
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
                    if ((Name.Length < 4) || (Email.Length < 4) || (Password.Length < 6) || (PasswordRepeated.Length < 4)
                        || (Phone.Length < 4) || (PinCode.Length < 4))
                    {
                        rv = false;
                    }
                    else
                    {
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
