using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AsthmaApp.Repository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly string _connectionString;

        public LocationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Location?> GetLocationAsync(string city, string country)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "SELECT * FROM \"Location\" WHERE \"IsActive\" = TRUE AND \"City\" ILIKE @city AND \"Country\" ILIKE @country;";

                await connection.OpenAsync();

                var location = await connection.QuerySingleOrDefaultAsync<Location>(commandText, new { 
                    City = city,
                    Country = country
                });

                await connection.CloseAsync();

                return location;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> InsertLocationAsync(Location location)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "INSERT INTO \"Location\" " +
                                        "(\"Id\", \"City\", \"Country\", \"Longitude\", \"Latitude\", \"IsActive\", \"DateCreated\", \"DateUpdated\") " +
                                        "VALUES (@id, @city, @country, @longitude, @latitude, @isActive, @dateCreated, @dateUpdated);";

                await connection.OpenAsync();

                var result = await connection.ExecuteAsync(commandText, new
                {
                    id = location.Id,
                    city = location.City,
                    country = location.Country,
                    longitude = location.Longitude,
                    latitude = location.Latitude,
                    isActive = location.IsActive,
                    dateCreated = location.DateCreated,
                    dateUpdated = location.DateUpdated
                });

                await connection.CloseAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
