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
using RestSharp.Portable.HttpClient;
using RestSharp.Portable;
using System.Net;

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


            //var encMgr = Factory.Instance.GetEncryptionManager();
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
            await Send.SendData("AddAppUserProfile", new string[]
            {
               "name", systerUser.Name,"password", systerUser.PreferredName,"password", systerUser.Email,
                               "password", systerUser.DateOfBirth.ToString(),"password", systerUser.Password,"password", systerUser.Phone,"password", systerUser.PinCode
            });
        }

        // register system user
        public async Task<bool> RegisterWeb(SystemUser sys)
        {
            using (var client = new RestClient("https://apps"))
            {
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", param, ParameterType.RequestBody);

                try
                {
                    var resp = await client.Execute(request);
                    if (resp.StatusCode == HttpStatusCode.OK)
                        return resp.Content.Contains("error") ? false : true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("Register exception : {0}--{1}", ex.Message, ex.InnerException);
#endif
                    return false;
                }
            }
        }

        public UserProfile LoginUser(string name, string password)
        {
            using (var client = new HttpClient())
            {
                var postData = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("", name),
                    new KeyValuePair<string, string>("", password)
                });

                var userProfile = new UserProfile();

                try
                {
                    var result = client.PostAsync("https://apps", postData).Result;
                    string resultContent = result.Content.ReadAsStringAsync().Result;
                    if (resultContent.ToLowerInvariant().Contains("failure") ||
                        resultContent.ToLowerInvariant().Contains("error") ||
                        resultContent.ToLowerInvariant().Contains("lockout") ||
                        resultContent.ToLowerInvariant().Contains("verification"))
                    {
                        userProfile.LogFail = resultContent;
                        return userProfile;
                    }
                    else
                    {
                        /* convert to user profile object */
                        userProfile = JsonConvert.DeserializeObject<UserProfile>(resultContent);
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
