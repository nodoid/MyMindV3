using System;
using SQLite.Net;
namespace MvvmFramework
{
    public interface ISqLiteConnectionFactory
    {
        SQLiteConnection GetConnection();
    }
}
