using MvvmFramework.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MvvmFramework.Webservices
{
    class UsersWebservice
    {
        public async Task<IEnumerable<Resources>> GetResources()
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(string.Format("{0}/api/MyMind/GetResources", Constants.BaseTestUrl));

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(string.Format("{0}/api/MyMind/GetResources", Constants.BaseTestUrl)).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Resources>>(responseString);
                }

                return null;
            }
        }

        public async Task<IEnumerable<Appointment>> GetPatientAppointments(string clientId, string hcp)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(hcp))
                return null;

            var url = string.Format("{0}/api/MyMind/GetAppointmentsByClientAndHCP?clientId={1}&hcpCode={2}",
                                    Constants.BaseTestUrl, clientId, hcp);

            var encMgr = Factory.Instance.GetEncryptionManager();
            IEnumerable<Appointment> encryptions = null;

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    var encryptionJson = await response.Content.ReadAsStringAsync();
                    encryptions = JsonConvert.DeserializeObject<IEnumerable<Appointment>>(encryptionJson);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("get appts exception - {0}::{1}", ex.Message, ex.InnerException);
#endif
            }

            //return encMgr.DecryptAppointments(encryptions);
            return encryptions;
        }

        public async Task UpdateSystemUser(SystemUser sysUser)
        {
            await Send.SendData("UpdateAppUserProfile", new string[]
            {
                "guid", sysUser.Guid,"name", sysUser.Name,"email", sysUser.PreferredName,
                "dob", sysUser.DateOfBirth.ToString(),"phone", sysUser.Phone,"referralreason", sysUser.ReferralReason,"likes",  sysUser.Likes,
                "dislikes",  sysUser.Dislikes,"goals",  sysUser.Goals
            });
        }

        // add system user
        public async Task AddSystemUser(SystemUser systerUser)
        {
            // ADD TO MYMIND DB - MSSQL
            await Send.SendData("AddAppUserProfile", new string[]
            {
               "name", systerUser.Name,"password", systerUser.PreferredName,"password", systerUser.Email,
                               "password", systerUser.DateOfBirth.ToString(),"password", systerUser.Password,"password", systerUser.Phone,"password", systerUser.PinCode
            });
        }

        // register system user
        public UserProfile RegisterWeb(SystemUser systerUser)
        {
            //Send.SendData("RegisterAppUser", new string[] { }).ConfigureAwait(true);
            using (var client = new HttpClient())
            {
                //App.Self.NetSpinner.NetworkSpinner(true, "Registering new user", "Please wait");
                var postData = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("name", systerUser.Name),
                    new KeyValuePair<string,string>("preferredName", systerUser.PreferredName),
                    new KeyValuePair<string, string>("dob", systerUser.DateOfBirth),
                        new KeyValuePair<string, string>("email", systerUser.Email),
                        new KeyValuePair<string, string>("phone", systerUser.Phone),
                        new KeyValuePair<string, string>("password", systerUser.Password),
                        new KeyValuePair<string, string>("pincode",  systerUser.PinCode),
                    new KeyValuePair<string, string>("postcode",  systerUser.PostCode)
                    });

                try
                {
                    UserProfile userProfile = null;
                    var result = client.PostAsync("https://apps.nelft.nhs.uk/MyMind/Account/RegisterAppUser", postData).Result;
                    var resultContent = result.Content.ReadAsStringAsync().Result;

                    if (resultContent == "\"success\"")
                    {
                        userProfile = LoginUser(systerUser.Email, systerUser.Password);
                    }
                    //App.Self.NetSpinner.NetworkSpinner(false, "", "");
                    return userProfile;
                }
                catch (AggregateException)
                {
                    //App.Self.NetSpinner.NetworkSpinner(false, "", "");
                    return null;
                }
            }
        }

        public UserProfile LoginUser(string name, string password)
        {
            using (var client = new HttpClient())
            {
                var postData = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("useremail", name),
                    new KeyValuePair<string, string>("userpasswd", password)
                });

                try
                {
                    var result = client.PostAsync("https://apps.nelft.nhs.uk/MyMind/Account/LoginAppUser", postData).Result;
                    string resultContent = result.Content.ReadAsStringAsync().Result;
                    if (resultContent.ToLowerInvariant().Contains("failure") ||
                        resultContent.ToLowerInvariant().Contains("error") ||
                        resultContent.ToLowerInvariant().Contains("lockout"))
                    {
                        UserProfile userProfile = null;
                        return userProfile;
                    }
                    else
                    {
                        /* convert to user profile object */
                        var userProfile = JsonConvert.DeserializeObject<UserProfile>(resultContent);
                        return userProfile;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("Exception thrown creating a user - {0}::{1}", ex.Message, ex.InnerException);
#endif
                    return null;
                }
            }
        }
    }
}
