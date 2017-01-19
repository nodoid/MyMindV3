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

            /*
            AllRegisteredUsersListView.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null) return; // don't do anything if we just de-selected the row

                await DisplayAlert("Selected a chat client", "You selected " + "test", "OK");
                ((ListView)sender).SelectedItem = null; // de-select the row
            };*/
        }
    }
}
