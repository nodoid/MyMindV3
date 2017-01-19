using Android.Content;
using MyMindV3.Droid;
using Xamarin.Forms;
using Android.App;
using System.Linq;
using System.IO;
using Android.OS;
using Plugin.Permissions;
using Android.Content.PM;
using System;
using Android.Content.Res;

[assembly: Dependency(typeof(FileLauncher))]
namespace MyMindV3.Droid
{
    public class FileContentProvider : ContentProvider
    {
        public override int Delete(Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            return 0;
        }

        public override string GetType(Android.Net.Uri uri)
        {
            return null;
        }

        public override Android.Net.Uri Insert(Android.Net.Uri uri, ContentValues values)
        {
            return null;
        }

        public override bool OnCreate()
        {
            return false;
        }

        public override Android.Database.ICursor Query(Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            return null;
        }

        public override int Update(Android.Net.Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            return 0;
        }

        public override Android.OS.ParcelFileDescriptor OpenFile(Android.Net.Uri uri, string mode)
        {
            var file = new Java.IO.File(uri.Path);
            return ParcelFileDescriptor.Open(file, ParcelFileMode.ReadOnly);
        }
    }

    public class FileLauncher : IFileAndroid
    {
        public void launchfile(string filename, string type)
        {
            var permission = CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage).Result;
            if (permission != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage).ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        if (CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage).Result == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                        {
                            CopyAndRun(filename, type);
                        }
                    }
                });
            }
            else
                CopyAndRun(filename, type);
        }

        void CopyAndRun(string filename, string type)
        {
            var bytes = File.ReadAllBytes(filename);
            var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.Path + "/tmp." + filename.Split('.').Last();
            File.WriteAllBytes(externalPath, bytes);
            //var uri = Android.Net.Uri.Parse(MainActivity.Active.PackageName + "/" + externalPath);
            var file = new Java.IO.File(externalPath);
            try
            {
                //var destPath = Android.OS.Environment.ExternalStorageDirectory.Path + "/" + filename.Split('/').Last();
                //File.Copy(filename, destPath, true);
                //var file = new Java.IO.File(filename);
                //file.SetReadable(true);
                var intent = new Intent(Intent.ActionView);
                intent.AddFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                intent.AddFlags(ActivityFlags.GrantPrefixUriPermission);
                intent.SetDataAndType(Android.Net.Uri.FromFile(file), type);
                Forms.Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                var builder = new AlertDialog.Builder(Forms.Context);
                builder.SetTitle("Unable to launch file");
                builder.SetMessage(string.Format("Unable to launch {0}. Please visit the PlayStore to install an application capable of displaying this file.", filename.Split('/').Last()));
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate
                {
                });
                builder.Show();
            }
            /*catch (ActivityNotFoundException)
            {
                var builder = new AlertDialog.Builder(Forms.Context);
                builder.SetTitle("Unable to launch file");
                builder.SetMessage(string.Format("Unable to launch {0}. Please visit the PlayStore to install an application capable of displaying this file.", filename.Split('/').Last()));
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate
                {
                });
                builder.Show();
            }*/
        }
    }
}
