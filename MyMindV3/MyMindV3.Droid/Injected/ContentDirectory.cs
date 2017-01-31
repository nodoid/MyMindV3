using System;
using System.IO;
using System.Linq;
using Android.Content;
using Android.Widget;
using MyMindV3.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(ContentDir))]
namespace MyMindV3.Droid
{
    public class ContentDir : IContent
    {
        public string ContentDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public long FileSize(string filename)
        {
            return new FileInfo(filename).Length;
        }

        public string PicturesDirectory()
        {
            var picsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (!Directory.Exists(Path.Combine(picsDir, "Directory")))
                Directory.CreateDirectory(Path.Combine(picsDir, "Directory"));
            return picsDir;
        }

        public bool FileExists(string filename)
        {
            var fi = new FileInfo(filename);
            return File.Exists(filename) && fi.Length > 0;
        }

        public void StoreFile(string id, Stream sr)
        {
            var filename = string.Format("{0}/{1}", App.Self.ContentDirectory, id);
            if (File.Exists(filename))
                File.Delete(filename);
            if (sr.Length > 0)
            {
                using (var fs = File.Create(filename))
                {
                    sr.CopyTo(fs);
                }
            }
        }

        public void StoreImageFile(string id, Stream sr, bool isUser = true)
        {
            var filename = string.Format("{0}/{1}.jpg", App.Self.PicturesDirectory, id);
            if (File.Exists(filename))
                File.Delete(filename);
            if (sr.Length > 0)
            {
                using (var fs = File.Create(filename))
                {
                    sr.CopyTo(fs);
                }
                if (!HasJpegHeader(filename))
                    File.Delete(filename);
                else
                {
                    if (isUser)
                    {
                        App.Self.UserSettings.SaveSetting<string>("UserImage", filename, SettingType.String);
                        App.Self.UserSettings.SaveSetting<string>("UserImageDatestamp", DateTime.Now.ToString("g"), SettingType.String);
                    }
                    else
                    {
                        App.Self.UserSettings.SaveSetting<string>("ClinicianImage", filename, SettingType.String);
                        App.Self.UserSettings.SaveSetting<string>("ClinicianImageDatestamp", DateTime.Now.ToString("g"), SettingType.String);
                    }
                }
            }
        }

        public void LaunchFile(string filename)
        {
            try
            {
                var filepath = string.Format("{0}/{1}", App.Self.ContentDirectory, filename);
                var targetUri = Android.Net.Uri.FromFile(new Java.IO.File(filepath));
                var intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(targetUri, filename.Split('.').LastOrDefault());
                Forms.Context.StartActivity(intent);
            }
            catch (Exception)
            {
                Toast.MakeText(Forms.Context, string.Format("I am sorry but this file ({0}) cannot be displayed on your phone", filename), ToastLength.Short).Show();
            }
        }

        public byte[] LoadedFile(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        bool HasJpegHeader(string filename)
        {
            using (var br = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                var soi = br.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
                var marker = br.ReadUInt16(); // JFIF marker (FFE0) or EXIF marker(FF01)

                return soi == 0xd8ff && (marker & 0xe0ff) == 0xe0ff;
            }
        }
    }
}

