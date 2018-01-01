using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System;
using Newtonsoft.Json;
using RestSharp.Portable.HttpClient;
using RestSharp.Portable;
using MvvmFramework.Helpers;
using MvvmFramework.Models;
using System.Collections.Generic;
using System.Net;

#if DEBUG
using System.Diagnostics;
#endif

namespace MvvmFramework
{
    public class Send
    {
        static ManualResetEvent allDone = new ManualResetEvent(false);
        static bool fromFiles = false;
        static CookieContainer cookieContainer = new CookieContainer();

        public static async Task<List<T>> GetPostListObject<T>(string apiToUse, params string[] data)
        {
            var list = new List<T>();

            var url = string.Format("{0}{1}", Constants.BaseTestUrl, apiToUse);
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            for (var i = 0; i < data.Length; i += 2)
            {
                request.AddHeader(data[i].ToLowerInvariant(), data[i + 1]);
            }

            try
            {
                var response = await client.Execute(request);
                if (!string.IsNullOrEmpty(response.Content))
                    list = JsonConvert.DeserializeObject<List<T>>(response.Content);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Exception getting a list<T> - {0}--{1}", ex.Message, ex.InnerException);
#endif
            }

            return list;
        }

        public static async Task<string> SendData(string apiToUse, params string[] data)
        {
            var rv = string.Empty;

            var url = string.Format("{0}/{1}", Constants.BaseTestUrl, apiToUse);

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            for (var i = 0; i < data.Length; i += 2)
            {
                request.AddHeader(data[i].ToLowerInvariant(), data[i + 1]);
            }
            try
            {
                var response = await client.Execute(request);
                rv = response.Content;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Send data exception : {0}--{1}", ex.Message, ex.InnerException);
#endif
            }
            return rv;
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
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                var para = string.Empty;
                for (var i = 0; i < data.Length; i += 2)
                {
                    //request.AddHeader(data[i].ToLowerInvariant(), data[i + 1]);
                    para += string.Format("{0}={1}&", data[i].ToLowerInvariant(), data[i + 1]);
                }
                para.TrimEnd('&');
                request.AddParameter("application/x-www-form-urlencoded", para, ParameterType.RequestBody);
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
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static async Task<string> UploadPicture(Stream data, string filename, string guid, string authtoken, string type, bool fromfile = false)
        {
            fromFiles = fromfile;
            var url = string.Format("{0}/api/MyMind/UploadProfilePicture", Constants.BaseTestUrl);
            var filesize = data.Length;
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddFile("files", ReadFully(data), filename, "image/jpeg");
                var resp = await client.Execute(request);
                return resp.Content;
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("Exception uploading {0}--{1}", e.Message, e.InnerException);
#endif
                return "-1";
            }
        }

        public static async Task<Ratings> Rated(string apiToUse, int resourceId, params string[] data)
        {
            var url = string.Format("{0}/{1}", Constants.BaseTestUrl, apiToUse);
            Ratings rated = null;

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                for (var i = 0; i < data.Length; i += 2)
                {
                    request.AddHeader(data[i].ToLowerInvariant(), data[i + 1]);
                }
                var response = await client.Execute(request);
                rated = JsonConvert.DeserializeObject<Ratings>(response.Content);
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("Exception - {0}::{1}", e.Message, e.InnerException);
#endif
            }
            return rated;
        }

        public static async Task ReportBrokenLink(string apiToUse, params string[] data)
        {
            var url = string.Format("{0}/{1}", Constants.BaseTestUrl, apiToUse);
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                for (var i = 0; i < data.Length; i += 2)
                {
                    request.AddHeader(data[i].ToLowerInvariant(), data[i + 1]);
                }
                var response = await client.Execute(request);
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("Exception - {0}::{1}", e.Message, e.InnerException);
#endif
            }
        }


    }
}

