using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AsthmaApp.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;

        public RoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Role?> GetRoleByIdAsync(Guid id)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "SELECT * FROM \"Role\" WHERE \"IsActive\" = TRUE AND \"Id\" = @id";

                await connection.OpenAsync();

                var role = await connection.QuerySingleOrDefaultAsync<Role>(commandText, new { Id = id });

                await connection.CloseAsync();

                return role;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Role?> GetRoleByNameAsync(string name)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "SELECT * FROM \"Role\" WHERE \"Name\" ILIKE @name";

                await connection.OpenAsync();

                var role = await connection.QuerySingleOrDefaultAsync<Role>(commandText, new { Name = name });

                await connection.CloseAsync();

                return role;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
