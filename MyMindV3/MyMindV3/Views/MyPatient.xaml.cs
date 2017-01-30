using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MyMindV3.Languages;
using MvvmFramework.ViewModel;
using MvvmFramework.Models;
using MvvmFramework;

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
            ViewModel.IsConnected = App.Self.IsConnected;
            FillData().ConfigureAwait(true);
        }

        void Handle_SelectedIndexChanged(object sender, EventArgs e)
        {
            var text = ((Picker)pickPatient).SelectedIndex;
            if (text != 0)
            {
                var id = ConnectionsData.FirstOrDefault(w => w.Name == ConnectionsData[text - 1].Name);
                var s = Send.SendData<List<UserProfile>>("api/MyMind/GetConnectionsProfile", "ClinicianGUID",
                    ViewModel.ClinicianUser.ClinicianGUID, "AuthToken", ViewModel.ClinicianUser.APIToken,
                                                   "ClientGUID", id.ClientGUID).ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        var ss = t.Result[0];
                        if (ss != null)
                        {
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
                });
            }
        }

        async Task FillData()
        {
            await Send.SendData<List<Connections>>("api/MyMind/GetClinicianConnections", "ClinicianGUID",
                ViewModel.ClinicianUser.ClinicianGUID, "AuthToken", ViewModel.SystemUser.APIToken).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    ConnectionsData.AddRange(t.Result);
                    if (ConnectionsData.Count != 0)
                    {
                        var names = ConnectionsData.Select(w => w.Name).ToList();
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            pickPatient.Items.Add(Langs.MyPatient_Select);
                            foreach (var n in names)
                                pickPatient.Items.Add(n);
                        });
                    }
                }
            });
        }

        void UpdateSessionTimeOut()
        {
            ViewModel.UpdateSessionExpirationTime();
        }
    }
}
