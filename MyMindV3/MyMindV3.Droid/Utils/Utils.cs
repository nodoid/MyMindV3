using Android.Content;
using Android.OS;
using Java.Sql;
using Java.Text;
using Android.Text.Format;
using Android.Net;
using Java.IO;
using Java.Lang;
using Android.Provider;
using Android.Database;
using System.Collections.Generic;

namespace MyMindV3.Droid
{
    public class Utils
    {
        public static Dictionary<string, int> msgtoTickList = new Dictionary<string, int>();

        public static string convertTimestampToDate(long timestamp)
        {
            var tStamp = new Timestamp(timestamp);
            SimpleDateFormat simpleDateFormat = null;
            if (DateUtils.IsToday(timestamp))
            {
                simpleDateFormat = new SimpleDateFormat("hh:mm a");
                return "Today " + simpleDateFormat.Format(tStamp);
            }
            else
            {
                simpleDateFormat = new SimpleDateFormat("dd-MM-yyyy hh:mm a");
                return simpleDateFormat.Format(tStamp);
            }
        }

        public static long correctTimestamp(long timestampInMessage)
        {
            long correctedTimestamp = timestampInMessage;

            if (timestampInMessage.ToString().Length < 13)
            {
                var difference = 13 - timestampInMessage.ToString().Length;
                var differenceValue = "1";
                for (var i = 0; i < difference; i++)
                {
                    differenceValue += "0";
                }
                correctedTimestamp = (timestampInMessage * int.Parse(differenceValue))
                        + (Java.Lang.JavaSystem.CurrentTimeMillis() % (int.Parse(differenceValue)));
            }
            return correctedTimestamp;
        }


        public static Uri getOutputMediaFile(int type, bool isChatroom, Context c)
        {
            var mediaStorageDir = new File(Environment.ExternalStorageDirectory, c.GetString(Resource.String.app_name));

            if (!mediaStorageDir.Exists())
            {
                if (!mediaStorageDir.Mkdirs())
                {
                    return null;
                }
            }

            // Create a media file name
            var timeStamp = new SimpleDateFormat("yyyyMMddHHmmss").Format(new Java.Util.Date());
            File mediaFile;
            if (type == 1)
            {
                var imageStoragePath = mediaStorageDir + " Images/";

                createDirectory((Java.Lang.String)imageStoragePath);
                mediaFile = new File(imageStoragePath + "IMG" + timeStamp + ".jpg");
            }
            else if (type == 2)
            {
                var videoStoragePath = mediaStorageDir + " Videos/";
                createDirectory((Java.Lang.String)videoStoragePath);
                mediaFile = new File(videoStoragePath + "VID" + timeStamp + ".mp4");
            }
            else
            {
                return null;
            }
            return Uri.FromFile(mediaFile);
        }

        public static void createDirectory(String filePath)
        {
            if (!new File((string)filePath).Exists())
            {
                new File((string)filePath).Mkdirs();
            }
        }


        public static string getPath(Uri uri, bool isImage)
        {
            if (uri == null)
            {
                return null;
            }
            string[] projection;
            string coloumnName, selection;
            if (isImage)
            {
                selection = MediaStore.Images.Media.InterfaceConsts.Id + "=?";
                coloumnName = MediaStore.Images.Media.InterfaceConsts.Data;
            }
            else
            {
                selection = MediaStore.Video.Media.InterfaceConsts.Id + "=?";
                coloumnName = MediaStore.Video.Media.InterfaceConsts.Data;
            }
            projection = new string[] { coloumnName };
            ICursor cursor;
            if (Build.VERSION.SdkInt > BuildVersionCodes.Kitkat)
            {
                // Will return "image:x*"
                var wholeID = DocumentsContract.GetDocumentId(uri);
                // Split at colon, use second item in the array
                var id = wholeID.Split(':')[1];
                // where id is equal to
                if (isImage)
                {
                    cursor = PreferenceHelper
                            .Context
                            .ContentResolver
                            .Query(MediaStore.Images.Media.ExternalContentUri, projection, selection,
                                    new string[] { id }, null);
                }
                else
                {
                    cursor = PreferenceHelper
                            .Context
                            .ContentResolver
                            .Query(MediaStore.Video.Media.ExternalContentUri, projection, selection, new string[] { id },
                                    null);
                }
            }
            else
            {
                cursor = PreferenceHelper.Context.ContentResolver.Query(uri, projection, null, null, null);
            }
            string path = string.Empty;
            try
            {
                var column_index = cursor.GetColumnIndex(coloumnName);
                cursor.MoveToFirst();
                path = cursor.GetString(column_index);
                cursor.Close();
            }
            catch (Exception e)
            {
#if DEBUG
                System.Console.WriteLine("Exception thrown : {0--{1}", e.Message, e.InnerException);
#endif
            }
            return path;
        }

    }
}