using Xamarin.Forms;
using MyMindV3.ViewModels;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using MyMindV3.Models;
using MyMindV3.Languages;

namespace MyMindV3.Views
{
    public partial class MyPatient : ContentPage
    {
        RootViewModel _rootVM;
        List<Connections> ConnectionsData = new List<Connections>();

        public MyPatient(RootViewModel rvm)
        {
            RootVM = rvm;
            InitializeComponent();
            FillData().ConfigureAwait(true);
        }

        void Handle_SelectedIndexChanged(object sender, EventArgs e)
        {
            var text = ((Picker)pickPatient).SelectedIndex;
            if (text != 0)
            {
                var id = ConnectionsData.FirstOrDefault(w => w.Name == ConnectionsData[text - 1].Name);
                var s = Send.SendData<List<UserProfile>>("api/MyMind/GetConnectionsProfile", "ClinicianGUID", RootVM.ClinicianUser.ClinicianGUID, "AuthToken", RootVM.ClinicianUser.APIToken,
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
            await Send.SendData<List<Connections>>("api/MyMind/GetClinicianConnections", "ClinicianGUID", RootVM.ClinicianUser.ClinicianGUID, "AuthToken", RootVM.SystemUser.APIToken).ContinueWith((t) =>
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

        public RootViewModel RootVM
        {
            get { return _rootVM; }
            set
            {
                if (value != _rootVM)
                {
                    _rootVM = value;
                    OnPropertyChanged("RootVM");
                }
            }
        }

        void UpdateSessionTimeOut()
        {
            _rootVM.UpdateSessionExpirationTime();
        }
    }
}
