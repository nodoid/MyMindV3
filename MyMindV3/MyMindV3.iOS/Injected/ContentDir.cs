using System;
using System.IO;
using MyMindV3.iOS;
using Xamarin.Forms;
using QuickLook;
using UIKit;
using System.Linq;
using Foundation;

[assembly: Dependency(typeof(ContentDir))]
namespace MyMindV3.iOS
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
            return File.Exists(filename);
        }

        public void StoreFile(string id, Stream sr)
        {
            var filename = string.Format("{0}/{1}.jpg", App.Self.PicturesDirectory, id);
            if (File.Exists(filename))
                File.Delete(filename);
            using (var fs = File.Create(filename))
            {
                sr.CopyTo(fs);
            }
        }

        public void StoreImageFile(string id, Stream sr, bool isUser)
        {
            var filename = string.Format("{0}/{1}.jpg", App.Self.PicturesDirectory, id);
            if (File.Exists(filename))
                File.Delete(filename);
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

        public void LaunchFile(string filename)
        {
            try
            {
                var prevItem = new PreviewItemBundle(filename, App.Self.ContentDirectory);
                var prevController = new QLPreviewController
                {
                    DataSource = new PreviewController(prevItem)
                };
                var rootvc = UIApplication.SharedApplication.KeyWindow;
                var vc = rootvc.RootViewController?.ChildViewControllers.First().ChildViewControllers.Last().ChildViewControllers.First(); ;
                if (vc != null)
                {
                    var navcontroller = vc as UINavigationController;
                    using (var pool = new NSAutoreleasePool())
                    {
                        pool.InvokeOnMainThread(delegate ()
                            {
                                navcontroller.PushViewController(prevController, true);
                            });
                    }
                }
            }
            catch (Exception)
            {
                using (var pool = new NSAutoreleasePool())
                {
                    pool.InvokeOnMainThread(delegate ()
                        {
                            var s = new UIAlertView("Preview error", string.Format("I am sorry but this file ({0}) cannot be displayed on your phone", filename), null, "OK");
                            s.Show();
                        });
                }
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

