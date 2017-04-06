using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MvvmFramework.Helpers;
using MvvmFramework.Models;
using System.Linq;

namespace MvvmFramework
{
    public class GetData
    {
        static CookieContainer cookieContainer = new CookieContainer();

        public static async Task<string> GetPostcode(double lon, double lat)
        {
            var url = string.Format("https://api.postcodes.io/postcodes/lon/{0}/lat/{1}", lon, lat);
            var postcode = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    var res = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<Geolocation>(res);
                    postcode = obj.result[0].postcode;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Exception in GetData {0}-{1}", ex.Message, ex.InnerException);
#endif
            }

            return postcode;
        }

        static double ToKilometers(string val)
        {
            return Convert.ToDouble(val.Split(' ')[0]) / 1.6;
        }

        public static async Task<List<Postcodes>> GetSurroundingPostcodes(string myPostcode)
        {
            var url = string.Format("http://uk-postcodes.com/postcode/nearest?postcode={0}&miles=5&format=json", myPostcode);
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    var res = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<List<Postcodes>>(res);
                    return obj;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Exception in GetData {0}-{1}", ex.Message, ex.InnerException);
#endif
                return new List<Postcodes>();
            }

        }

        public static async Task<int> GetDistanceFromPostcodes(string myPostcode, string theirPostcode)
        {
            var url = string.Format("http://maps.googleapis.com/maps/api/distancematrix/json?origins={0}&destinations={1}&mode=driving&language=en-EN&sensor=false", myPostcode, theirPostcode);
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    var res = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<GooglePostcode>(res);
                    return obj.rows[0].elements[0].distance.value;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Exception in GetData {0}-{1}", ex.Message, ex.InnerException);
#endif
                return -1;
            }
        }

        public static async Task GetImage(string id, bool isUser = true)
        {
            var url = string.Format("{0}/api/MyMind/GetProfilePicture/{1}", Constants.BaseTestUrl, id.Split('/').Last().Split('.')[0]);
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
                            // DependencyService.Get<IContent>().StoreImageFile(id, str, isUser);
                            await new FileIO().SaveFile(id, str);
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
                            //DependencyService.Get<IContent>().StoreFile(filename, str);
                            await new FileIO().SaveFile(filename, str);
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
                        rv = JsonConvert.DeserializeObject<IEnumerable<Encryption>>(str);
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

        public static async Task<ResourcesModel> GetLocalNationalResources(string apiToUse, params string[] data)
        {
            ResourcesModel rm = null;

            var url = string.Format("{0}/api/MyMind/{1}", Constants.BaseTestUrl, apiToUse);


            try
            {
                using (var client = new HttpClient())
                {
                    using (var message = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        for (var i = 0; i < data.Length; i += 2)
                        {
                            message.Headers.Add(data[i].ToLowerInvariant(), data[i + 1]);
                        }
                        var response = client.SendAsync(message).Result;
                        var str = await response.Content.ReadAsStringAsync();
                        rm = JsonConvert.DeserializeObject<ResourcesModel>(str);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception getting data - {0}::{1}", ex.Message, ex.InnerException);
            }

            return rm;
        }

        /*public static async Task<IEnumerable<Resources>> GetLocalNationalResources(string apiToUse, params string[] data)
        {
            IEnumerable<Resources> rm = null;

            var url = string.Format("{0}/api/MyMind/{1}", Constants.BaseTestUrl, apiToUse);


            try
            {
                using (var client = new HttpClient())
                {
                    using (var message = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        for (var i = 0; i < data.Length; i += 2)
                        {
                            message.Headers.Add(data[i].ToLowerInvariant(), data[i + 1]);
                        }
                        var response = client.SendAsync(message).Result;
                        var str = await response.Content.ReadAsStringAsync();
                        var t = JsonConvert.DeserializeObject<ResourcesModel>(str);
                        rm = t?.Resources as IEnumerable<Resources>;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception getting data - {0}::{1}", ex.Message, ex.InnerException);
            }

            return rm;
        }*/
    }
}

