using System;
using System.IO;
using MyMindV3.iOS;
using GalaSoft.MvvmLight.Ioc;
using MvvmFramework;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinIOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteConnectionFactory))]
namespace MyMindV3.iOS
{
    public class SQLiteConnectionFactory : MvvmFramework.ISqLiteConnectionFactory
    {
        readonly string Filename = "mymindv3.db";

        public SQLiteConnection GetConnection()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            path = Path.Combine(path, Filename);

            SimpleIoc.Default.Register<ISqLiteConnectionFactory, SQLiteConnectionFactory>();
            return new SQLiteConnection(SQLitePlatform, path);
        }

        public ISQLitePlatform SQLitePlatform
        {
            get { return new SQLitePlatformIOS(); }
        }
    }
}

