using MyMindV3.iOS;
using System;
using System.IO;
using Xamarin.Forms;
using SQLite.Net.Platform.XamarinIOS;
using SQLite.Net;

[assembly: Dependency(typeof(SQLite_iOS))]
namespace MyMindV3.iOS
{
    public class SQLite_iOS : ISQLite
    {
        #region ISQLite implementation
        public SQLiteConnection GetConnection()
        {
            var fileName = "SystemUsers.db3";
            var libraryPath = Path.Combine(App.Self.ContentDirectory, "..", "Library");
            var path = Path.Combine(libraryPath, fileName);

            var conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformIOS(), path);
            return conn;
        }
        #endregion
    }
}