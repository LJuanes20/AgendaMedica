using System.Data.SqlClient;

namespace AMAPI.Infrastructure
{
    public class DbContext : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _sqlConnection;

        public DbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            _sqlConnection.Open();
        }


        public SqlConnection Connection()
        {
            if (_sqlConnection.State != System.Data.ConnectionState.Open)
                _sqlConnection.Open();

            return _sqlConnection;
        }

        public void Dispose()
        {
            if (_sqlConnection is null)
                return;

            _sqlConnection.Dispose();
        }
    }
}
