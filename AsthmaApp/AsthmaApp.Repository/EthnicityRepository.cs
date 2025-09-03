using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AsthmaApp.Repository
{
    public class EthnicityRepository : IEthnicityRepository
    {
        private readonly string _connectionString;

        public EthnicityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Ethnicity>> GetAllEthnicitiesAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "SELECT * FROM \"Ethnicity\" WHERE \"IsActive\" = TRUE";

                await connection.OpenAsync();

                var entities = await connection.QueryAsync<Ethnicity>(commandText);

                await connection.CloseAsync();

                return entities.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
