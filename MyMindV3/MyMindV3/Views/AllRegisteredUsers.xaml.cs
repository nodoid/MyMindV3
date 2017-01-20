using MvvmFramework.ViewModel;
using System.Linq;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class AllRegisteredUsers : ContentPage
    {
        AllRegisteredUserViewModel ViewModel => App.Locator.AllRegisteredUsers;

        public AllRegisteredUsers()
        {
            InitializeComponent();
            BindingContext = ViewModel;
            var users = ViewModel.GetSystemUsers;
            AllRegisteredUsersListView.ItemsSource = users;
            ViewModel.AllUsersTotal = users.Count();
        }
    }
}
