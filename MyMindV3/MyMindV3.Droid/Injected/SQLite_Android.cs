using System;
using System.IO;
using Xamarin.Forms;
using MyMindV3.Droid;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;

[assembly: Dependency(typeof(SQLite_Android))]
namespace MyMindV3.Droid
{
    public class SQLite_Android : ISQLite
    {
        #region ISQLite implementation
        public SQLiteConnection GetConnection()
        {
            var fileName = "SystemUsers.db3";
            var path = Path.Combine(App.Self.ContentDirectory, fileName);

            var conn = new SQLiteConnection(new SQLitePlatformAndroid(), path);
            return conn;
        }
        #endregion
    }
}