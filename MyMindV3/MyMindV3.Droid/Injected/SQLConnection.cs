using System;
using System.IO;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinAndroid;
using MyMindV3.Droid;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Ioc;
using MvvmFramework;

[assembly: Dependency(typeof(SQLConnection))]
namespace MyMindV3.Droid
{
    public class SQLConnection : MvvmFramework.ISqLiteConnectionFactory
    {
        readonly string Filename = "mymindv3.db";

        public SQLiteConnection GetConnection()
        {
            var path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            path = Path.Combine(path, Filename);

            SimpleIoc.Default.Register<ISqLiteConnectionFactory, SQLConnection>();

            return new SQLiteConnection(SQLitePlatform, path);
        }

        public ISQLitePlatform SQLitePlatform
        {
            get { return new SQLitePlatformAndroid(); }
        }
    }
}
