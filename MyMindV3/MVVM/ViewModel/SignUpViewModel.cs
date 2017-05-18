using System;
using MvvmFramework.Models;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Command;
using MvvmFramework.Webservices;
using MvvmFramework.Interfaces;

namespace MvvmFramework.ViewModel
{
    public class SignUpViewModel : BaseViewModel
    {
        IRepository _database;
        INavigationService navService;
        IDialogService dialogService;
        IConnectivity connectService;
        string DateOfBirth;

        public SignUpViewModel(INavigationService navigation, IRepository repo, IDialogService dialog, IConnectivity con)
        {
            navService = navigation;
            _database = repo;
            dialogService = dialog;
            connectService = con;

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
                            if (connectService.IsConnected)
                            {
                                if (HasValidInput)
                                {
                                    var systemUser = new SystemUser
                                    {
                                        Name = Uri.EscapeUriString(Name),
                                        PreferredName = Uri.EscapeUriString(PreferredName),
                                        DateOfBirth = Uri.EscapeUriString(DateOfBirth),
                                        Email = Email.Replace("@", "%40"),
                                        Phone = Uri.EscapeUriString(Phone),
                                        Password = Uri.EscapeUriString(Password),
                                        PinCode = Uri.EscapeUriString(PinCode),
                                        PostCode = Uri.EscapeUriString(PostCode.ToUpperInvariant())
                                    };

                                    await new UsersWebservice().RegisterWeb(systemUser).ContinueWith(async (t) =>
                                    {
                                        if (t.IsCompleted && (!t.IsCanceled && !t.IsFaulted))
                                        {
                                            if (t.Result)
                                                await dialogService.ShowMessage(GetMessage("RegUser_Completed_Message"), GetMessage("RegUser_Completed"), "OK", () => navService.GoBack());
                                            else
                                                await dialogService.ShowMessage(GetErrorMessage("Registration_ErrorMessage"), GetErrorTitle("Gen_Error"), "OK", () => navService.GoBack());
                                        }
                                    });
                                }
                                else
                                    await dialogService.ShowMessage(NetworkErrors[1], NetworkErrors[0], "OK", () => navService.GoBack());
                                createSubmitCommand = null;
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
