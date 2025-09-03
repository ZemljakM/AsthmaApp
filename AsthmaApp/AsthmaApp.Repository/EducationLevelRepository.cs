using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AsthmaApp.Repository
{
    public class EducationLevelRepository : IEducationLevelRepository
    {
        private readonly string _connectionString;

        public EducationLevelRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<EducationLevel>> GetAllEducationLevelsAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "SELECT * FROM \"EducationLevel\" WHERE \"IsActive\" = TRUE";

                await connection.OpenAsync();

                var entities = await connection.QueryAsync<EducationLevel>(commandText);

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
