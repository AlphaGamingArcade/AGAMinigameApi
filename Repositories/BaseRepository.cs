using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace AGAMinigameApi.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        protected async Task<DataTable> SelectQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            var dataTable = new DataTable();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);

            return dataTable;
        }
    }
}