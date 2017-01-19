using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using SQLite.Net;
using System.Diagnostics;

namespace MvvmFramework
{
    public class SystemUserDB
    {
        private SQLiteConnection _sqlConnection;

        public SystemUserDB()
        {
            _sqlConnection = DependencyService.Get<ISQLite>().GetConnection();
            _sqlConnection.CreateTable<SystemUser>();
        }

        // get all system users
        public IEnumerable<SystemUser> GetSystemUsers()
        {
            return (from t in _sqlConnection.Table<SystemUser>()
                    select t).ToList();
        }

        // get specific system user
        public SystemUser GetSystemUser(int id)
        {
            return _sqlConnection.Table<SystemUser>().FirstOrDefault(t => t.UserID == id);
        }

        // delete system user - this is for dev purposes
        public void DeleteSystemUser(int id)
        {
            _sqlConnection.Delete<SystemUser>(id);
        }

        // update system user
        
    }
}
