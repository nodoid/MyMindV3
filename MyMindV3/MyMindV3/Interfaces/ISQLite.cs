using SQLite.Net;

namespace MyMindV3
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
