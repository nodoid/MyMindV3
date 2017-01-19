using SQLite.Net;
using Xamarin.Forms;
using MyMindV3.Models;

namespace MyMindV3
{
    public class DataCategory
    {
        SQLiteConnection _sqlConnection;

        public DataCategory()
        {
            _sqlConnection = DependencyService.Get<ISQLite>().GetConnection();
            _sqlConnection.CreateTable<ResourceModel>();
            PopulateTable();
        }

        void PopulateTable()
        {
            var data = DependencyService.Get<IDataReader>().LoadDataFile();
            if (data.Count != 0)
            {
                foreach (var d in data)
                {
                    _sqlConnection.InsertOrIgnore(d);
                }
                _sqlConnection.Commit();
            }
        }
    }
}

