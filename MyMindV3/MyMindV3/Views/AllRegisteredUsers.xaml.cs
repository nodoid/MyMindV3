using MyMindV3.Data;
using MyMindV3.Models;
using MyMindV3.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMindV3.Views
{
    public partial class AllRegisteredUsers : ContentPage
    {
        private SystemUserDB _systemUsersDB;
        private AllUsersViewModel _allUsersVM;

        public AllRegisteredUsers()
        {
            InitializeComponent();

            _systemUsersDB = new SystemUserDB();

            IEnumerable<SystemUser> _users = _systemUsersDB.GetSystemUsers();

            AllRegisteredUsersListView.ItemsSource = _users;

            _allUsersVM = new AllUsersViewModel();
            BindingContext = _allUsersVM;

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
