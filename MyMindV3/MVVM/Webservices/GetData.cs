using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MvvmFramework
{
    public class GetData
    {
        static CookieContainer cookieContainer = new CookieContainer();


        public static async Task GetImage(string id, bool isUser = true)
        {
            var url = string.Format("{0}/api/MyMind/GetProfilePicture/{1}", Constants.BaseTestUrl, id);
            try
            {
                using (var client = new HttpClient())
                {
                    using (var message = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        var response = client.GetAsync(url).Result;
                        var str = await response.Content.ReadAsStreamAsync();
                        if (str.Length > 1024)
                        {
                            DependencyService.Get<IContent>().StoreImageFile(id, str, isUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception getting data - {0}::{1}", ex.Message, ex.InnerException);
            }
        }

        public static async Task GetFile(string id, string guid, string auth, string filename)
        {
            var url = string.Format("{0}/api/MyMind/GetFileData", Constants.BaseTestUrl);

            try
            {
                using (var client = new HttpClient())
                {
                    using (var message = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        message.Headers.Add("UserGUID", guid);
                        message.Headers.Add("AuthToken", auth);
                        message.Headers.Add("FileId", id);
                        var response = client.SendAsync(message).Result;
                        var str = await response.Content.ReadAsStreamAsync();
                        if (str.Length > 1024)
                        {
                            DependencyService.Get<IContent>().StoreFile(filename, str);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception getting data - {0}::{1}", ex.Message, ex.InnerException);
            }

        }

        public static async Task<T> GetSingleObject<T>(string apiToUse, params string[] data)
        {
            var rv = Activator.CreateInstance<T>();

            var url = Constants.BaseTestUrl;
            foreach (var d in data)
                url += string.Format("/{0}", d);

            try
            {
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (var client = new HttpClient(handler))
                    {
                        using (var message = new HttpRequestMessage(HttpMethod.Get, url))
                        {
                            var response = client.GetAsync(url).Result;
                            var str = await response.Content.ReadAsStringAsync();
                            rv = JsonConvert.DeserializeObject<T>(str);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("Exception getting data - {0}::{1}", ex.Message, ex.InnerException);
            }

            return rv;
        }

        public static async Task<List<T>> GetListObject<T>(string apiToUse, params string[] data)
        {
            var list = new List<T>();

            var url = string.Format("{0}{1}", Constants.BaseTestUrl, apiToUse);
            if (data.Length != 0)
            {
                foreach (var d in data)
                    url += string.Format("/{0}", d);
            }

            try
            {
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (var client = new HttpClient(handler))
                    {
                        using (var message = new HttpRequestMessage(HttpMethod.Get, url))
                        {
                            var response = client.GetAsync(url).Result;
                            var str = await response.Content.ReadAsStringAsync();
                            if (!str.Contains("html"))
                                list.AddRange(JsonConvert.DeserializeObject<List<T>>(str));
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("Exception getting data - {0}::{1}", ex.Message, ex.InnerException);
            }

            return list;
        }

        public static async Task<Encryption> GetSingleEncryptedObject(string apiToUse, params string[] data)
        {
            Encryption rv = null;

            var url = Constants.BaseUrl;
            foreach (var d in data)
                url += string.Format("/{0}", d);

            try
            {
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (var client = new HttpClient(handler))
                    {
                        using (var message = new HttpRequestMessage(HttpMethod.Get, url))
                        {
                            var response = client.GetAsync(url).Result;
                            var str = await response.Content.ReadAsStringAsync();
                            rv = JsonConvert.DeserializeObject<Encryption>(str);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("Exception getting data - {0}::{1}", ex.Message, ex.InnerException);
            }

            return rv;
        }

        public static async Task<IEnumerable<Encryption>> GetListEncryptedObjects(string apiToUse, params string[] data)
        {
            IEnumerable<Encryption> rv = null;
            var url = string.Format("{0}/{1}", Constants.BaseTestUrl, apiToUse);
            foreach (var d in data)
                url += string.Format("{0}&", d);

            url = url.Substring(0, url.LastIndexOf('&'));

            try
            {
                using (var client = new HttpClient())
                {
                    using (var message = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        var response = client.GetAsync(url).Result;
                        var str = await response.Content.ReadAsStringAsync();
                        rv = JsonConvert.DeserializeObject<IEnumerable<Classes.Encryption>>(str);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("Exception getting data - {0}::{1}", ex.Message, ex.InnerException);
            }
            catch (JsonReaderException ex)
            {
                Debug.WriteLine("Exception deserializing data - {0}::{1}", ex.Message, ex.InnerException);
            }
            return rv;
        }
    }
}

