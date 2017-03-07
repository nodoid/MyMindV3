using MvvmFramework.ViewModel;
using Xamarin.Forms;
using System.Collections.Generic;
using MyMindV3.Languages;

namespace MyMindV3.Views
{
    public partial class SignUpView : ContentPage
    {
        SignUpViewModel ViewModel => App.Locator.SignUp;

        public SignUpView()
        {
            InitializeComponent();

            BindingContext = ViewModel;

            ViewModel.Messages = new Dictionary<string, string> { { "RegUser_Completed_Message", Langs.RegUser_Completed_Message },
                {"RegUser_Completed", Langs.RegUser_Completed}};
            ViewModel.ErrorMessages = new Dictionary<string, string> { { "Registration_ErrorMessage", Langs.Registration_ErrorMessage } };
            ViewModel.ErrorTitles = new Dictionary<string, string> { { "Gen_Error", Langs.Gen_Error } };
        }
    }
}
