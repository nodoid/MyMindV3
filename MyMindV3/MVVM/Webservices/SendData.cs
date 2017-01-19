using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System;
using Newtonsoft.Json;
using RestSharp.Portable.HttpClient;
using RestSharp.Portable;

#if DEBUG
using System.Diagnostics;
#endif

namespace MvvmFramework
{
    public class Send
    {
        static ManualResetEvent allDone = new ManualResetEvent(false);
        static byte[] fileArr;
        static bool fromFiles = false;

        /*public static async Task<string> SendData(string apiToUse, params string[] data)
        {
            var rv = string.Empty;

            var url = string.Format("{0}/{1}", Constants.BaseTestUrl, apiToUse);

            var kvp = new List<KeyValuePair<string, string>>();

            for (var i = 0; i < data.Length; i += 2)
                kvp.Add(new KeyValuePair<string, string>(data[i], data[i + 1]));

            using (var client = new HttpClient())
            {
                var result = client.PostAsync(url, new FormUrlEncodedContent(kvp.ToArray())).Result;
                rv = await result.Content.ReadAsStringAsync();
            }

            return rv;
        }

        public static async Task<T> SendData<T>(string apiToUse, params string[] data)
        {
            var url = string.Format("{0}/{1}", Constants.BaseTestUrl, apiToUse);

            dynamic t;

            var kvp = new Dictionary<string, string>();

            for (var i = 0; i < data.Length; i += 2)
            {
                kvp.Add(data[i], data[i + 1]);
            }

            var dta = new FormUrlEncodedContent(kvp);
            dta.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            using (var client = new HttpClient())
            {
                var result = client.PostAsync(url, dta).Result;
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                var rv = result.Content.ReadAsStringAsync().Result;
                t = JsonConvert.DeserializeObject<T>(rv);
            }

            return t;
        }*/

        public static async Task<string> SendData(string apiToUse, params string[] data)
        {
            var rv = string.Empty;

            var url = string.Format("{0}/{1}", Constants.BaseTestUrl, apiToUse);

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            //request.AddHeader("content-type", "multipart/form-data");
            for (var i = 0; i < data.Length; i += 2)
            {
                request.AddHeader(data[i].ToLowerInvariant(), data[i + 1]);
            }
            var response = await client.Execute(request);
            rv = response.Content;
            return rv;

            /*var kvp = new List<KeyValuePair<string, string>>();

            for (var i = 0; i < data.Length; i += 2)
                kvp.Add(new KeyValuePair<string, string>(data[i], data[i + 1]));

            using (var client = new HttpClient())
            {
                var result = client.PostAsync(url, new FormUrlEncodedContent(kvp.ToArray())).Result;
                rv = await result.Content.ReadAsStringAsync();
            }

            return rv;*/
        }

        public static async Task<T> SendData<T>(string apiToUse, params string[] data)
        {
            var url = string.Format("{0}/{1}", Constants.BaseTestUrl, apiToUse);

            dynamic t;

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                //request.AddHeader("content-type", "multipart/form-data");
                for (var i = 0; i < data.Length; i += 2)
                {
                    request.AddHeader(data[i].ToLowerInvariant(), data[i + 1]);
                }
                var response = await client.Execute(request);
                t = JsonConvert.DeserializeObject<T>(response.Content);
                return t;
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("Exception - {0}::{1}", e.Message, e.InnerException);
#endif
                return default(T);
            }
            /*var kvp = new Dictionary<string, string>();

                   for (var i = 0; i < data.Length; i += 2)
                   {
                       kvp.Add(data[i], data[i + 1]);
                   }

                   var dta = new FormUrlEncodedContent(kvp);
                   dta.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

                   using (var client = new HttpClient())
                   {
                       var result = client.PostAsync(url, dta).Result;
                       //result.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                       var rv = result.Content.ReadAsStringAsync().Result;
                       t = JsonConvert.DeserializeObject<T>(rv);
                   }

                   return t;*/
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /*public static void UploadPicture(MediaFile source, string guid, bool fromfile = false)
        {
            fromFiles = fromfile;
            var Url = string.Format("{0}/api/MyMind/UploadProfilePicture/{1}", Constants.BaseTestUrl, guid);
            fileArr = null;
            try
            {
                using (var client = new HttpClient())
                {
                    using (var message = new HttpRequestMessage(HttpMethod.Post, Url))
                    {
                        using (var multi = new MultipartContent())
                        {
                            var data = new ByteArrayContent(ReadFully(source.GetStream()));
                            multi.Add(data);
                            //message.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                            var response = client.PostAsync(Url, multi).Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Exception getting data - {0}::{1}", ex.Message, ex.InnerException);
#endif
            }
        }*/

        public static async Task UploadPicture(string filename, string guid, bool fromfile = false)
        {
            fromFiles = fromfile;
            var url = string.Format("{0}/api/MyMind/UploadProfilePicture/{1}", Constants.BaseTestUrl, guid);
            var filesize = DependencyService.Get<IContent>().FileSize(filename);
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                //request.AddHeader("content-type", "multipart/form-data");
                //request.AddParameter("file", filename, ParameterType.GetOrPost);
                request.AddFile("files", DependencyService.Get<IContent>().LoadedFile(filename), filename, "image/jpeg");
                var resp = await client.Execute(request);
                var s = resp.Content;
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("Exception uploading {0}--{1}", e.Message, e.InnerException);
#endif
            }
        }

        public static bool HttpPost(MediaFile source, string guid, bool fromfile = false)
        {
            fromFiles = fromfile;
            var Url = string.Format("{0}/api/MyMind/UploadProfilePicture/{1}", Constants.BaseTestUrl, guid);
            fileArr = null;
            Device.BeginInvokeOnMainThread(() => App.Self.NetSpinner.NetworkSpinner(true, Langs.Data_UploadingTitle, Langs.Data_PleaseWait));
            try
            {
                using (var streamReader = new StreamReader(source.GetStream()))
                {
                    using (var memstream = new MemoryStream())
                    {
                        streamReader.BaseStream.CopyTo(memstream);
                        fileArr = memstream.ToArray();
                    }
                }

                var req = WebRequest.Create(Url) as HttpWebRequest;
                req.ContentType = "multipart/form-data";
                req.Method = "POST";

                req.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), req);
                allDone.WaitOne();
                if (!fromFiles)
                    App.Self.FileUploadedFine = true;
                else
                    App.Self.FileUploadedFileFine = true;
                return App.Self.FileUploadedFine;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Exception sending data - {0}:{1}", ex.Message, ex.InnerException);
#endif
                Device.BeginInvokeOnMainThread(() => App.Self.NetSpinner.NetworkSpinner(false, string.Empty, string.Empty));
                if (!fromFiles)
                    App.Self.FileUploadedFine = false;
                else
                    App.Self.FileUploadedFileFine = false;
                return App.Self.FileUploadedFine;
            }
        }

        static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            using (var postStream = request.EndGetRequestStream(asynchronousResult))
            {
                var chunk = fileArr.Length % 10000;
                var len = fileArr.Length - chunk;
                for (var i = 0; i < len; i += 10000)
                {
                    postStream.Write(fileArr, i, 10000);
                    App.Self.Uploaded += 10000;
                }
                postStream.Write(fileArr, len, chunk);
                App.Self.Uploaded += chunk;
            }
            fileArr = null;

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }

        static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;
            try
            {
                // End the operation
                var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                using (var streamResponse = response.GetResponseStream())
                {
                    using (var streamRead = new StreamReader(streamResponse))
                    {
                        var responseString = streamRead.ReadToEnd();
                        if (!fromFiles)
                            App.Self.FileUploadedFine = true;
                        else
                            App.Self.FileUploadedFileFine = true;
                    }
                }
                Device.BeginInvokeOnMainThread(() => App.Self.NetSpinner.NetworkSpinner(false, string.Empty, string.Empty));
                allDone.Set();
            }
            catch (Exception ex)
            {
                allDone.Set();
                Device.BeginInvokeOnMainThread(() => App.Self.NetSpinner.NetworkSpinner(false, string.Empty, string.Empty));
#if DEBUG
                Debug.WriteLine("Exception thrown : {0}--{1}", ex.Message, ex.InnerException);
#endif
                if (!fromFiles)
                    App.Self.FileUploadedFine = false;
                else
                    App.Self.FileUploadedFileFine = false;
            }
        }
    }
}

