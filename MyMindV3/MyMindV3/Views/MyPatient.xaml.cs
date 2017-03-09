using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using MyMindV3.Languages;
using MvvmFramework.ViewModel;
using MvvmFramework.Models;

namespace MyMindV3.Views
{
    public partial class MyPatient : ContentPage
    {
        MyPatientViewModel ViewModel => App.Locator.MyPatient;
        List<Connections> ConnectionsData = new List<Connections>();

        public MyPatient()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FillData();
            ViewModel.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "Connections")
                {
                    if (ConnectionsData != null)
                        ConnectionsData.Clear();
                    else
                        ConnectionsData = new List<Connections>();

                    ConnectionsData.AddRange(ViewModel.Connections);
                }

                if (e.PropertyName == "ConnectionNames")
                {
                    Device.BeginInvokeOnMainThread(() =>
                        {
                            pickPatient.Items.Add(Langs.MyPatient_Select);
                            foreach (var n in ViewModel.ConnectionNames)
                                pickPatient.Items.Add(n);
                        });
                }

                if (e.PropertyName == "UserProfile")
                {
                    if (ViewModel.UserProfile != null)
                    {
                        var ss = ViewModel.UserProfile;
                        Device.BeginInvokeOnMainThread(() =>
                            {
                                ProfilePreferredNameInput.Text = ss.Name;
                                ProfileDoBInput.Text = ss.DateOfBirth;
                                ProfilePhoneInput.Text = ss.ContactNumber;
                                RegPhoneInput.Text = ss.ReferralReason;
                                ProfileLikesInput.Text = ss.Likes;
                                ProfileDislikesInput.Text = ss.Dislikes;
                                ProfileGoalsInput.Text = ss.Goals;
                            });
                    }
                }
            };
        }

        void Handle_SelectedIndexChanged(object sender, EventArgs e)
        {
            var text = ((Picker)pickPatient).SelectedIndex;
            if (text != 0)
            {
                ViewModel.ConnectionId = ConnectionsData?.FirstOrDefault(w => w.Name == ConnectionsData[text - 1].Name).ClientGUID;
                ViewModel.UnencryptedGuid = ViewModel.ClinicianUser.ClinicianGUID;
                ViewModel.UnencryptedAuth = ViewModel.ClinicianUser.APIToken;
                ViewModel.GetUserProfile();
            }
        }

        void FillData()
        {
            ViewModel.AuthToken = App.Self.Encrypt.EncryptHcpString(ViewModel.SystemUser.APIToken);
            ViewModel.ClinicianGuid = App.Self.Encrypt.EncryptHcpString(ViewModel.ClinicianUser.ClinicianGUID);
            ViewModel.GetConnections();
        }

        void UpdateSessionTimeOut()
        {
            ViewModel.UpdateSessionExpirationTime();
        }
    }
}
