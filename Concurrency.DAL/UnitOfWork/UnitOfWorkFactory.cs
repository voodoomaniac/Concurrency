using System.Configuration;
using System.Data.SqlClient;

namespace Concurrency.DAL.UnitOfWork
{
    public static class UnitOfWorkFactory
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static AdoNetUnitOfWork Create()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();

            return new AdoNetUnitOfWork(connection, true);
        }
    }
}
