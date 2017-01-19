using MvvmFramework.Models;
using MvvmFramework.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class AllRegisteredUsers : ContentPage
    {
        MyClinicianViewModel ViewModel => App.Locator.MyClinician;

        public AllRegisteredUsers()
        {
            InitializeComponent();

            IEnumerable<SystemUser> _users = _systemUsersDB.GetSystemUsers();

            AllRegisteredUsersListView.ItemsSource = _users;
            BindingContext = ViewModel;

            _allUsersVM.AllUsersTotal = _users.Count();
        }
    }
}
