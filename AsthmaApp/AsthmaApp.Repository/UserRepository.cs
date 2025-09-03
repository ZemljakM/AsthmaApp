using AsthmaApp.Common;
using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Text;

namespace AsthmaApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "DELETE FROM \"User\" WHERE \"Id\" = @id;";

                await connection.OpenAsync();

                var rowsAffected = await connection.ExecuteAsync(commandText, new
                {
                    Id = id
                });

                await connection.CloseAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeactivateUserAsync(User user)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "UPDATE \"User\" SET" +
                    "\"IsActive\" = false," +
                    "\"DateUpdated\" = @dateUpdated," +
                    "\"UpdatedByUserId\" = @updatedByUserId " +
                    "WHERE \"Id\" = @id;";

                await connection.OpenAsync();

                var rowsAffected = await connection.ExecuteAsync(commandText, new
                {
                    id = user.Id,
                    dateUpdated = user.DateUpdated,
                    updatedByUserId = user.UpdatedByUserId
                });

                await connection.CloseAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<User>> GetAllUsersAsync(Filter filter, Paging paging, Sorting sorting)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT u.*, r.\"Id\" as Role_Id, r.\"Name\" FROM \"User\" u " +
                    "JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\" WHERE 1=1 ");

                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(filter?.SearchQuery))
                {
                    stringBuilder.Append("AND (u.\"FirstName\" ILIKE @search OR u.\"LastName\" ILIKE @search OR u.\"Email\" ILIKE @search) ");
                    parameters.Add("@search", $"%{filter.SearchQuery}%");
                }
                if (filter?.IsApproved != null)
                {
                    stringBuilder.Append("AND u.\"IsApproved\" = @isApproved ");
                    parameters.Add("@isApproved", filter.IsApproved);
                }

                stringBuilder.Append($" ORDER BY \"{sorting.OrderBy}\" {sorting.OrderDirection} " +
                    $"LIMIT @nextRows OFFSET @offset ");
                parameters.Add("@offset", paging.PageSize * (paging.PageNumber - 1));
                parameters.Add("@nextRows", paging.PageSize);

                await connection.OpenAsync();

                var lookup = new Dictionary<Guid, User>();

                await connection.QueryAsync<User, Role, User>(stringBuilder.ToString(),
                    (u, r) =>
                    {
                        if (!lookup.TryGetValue(u.Id, out var user))
                        {
                            lookup.Add(u.Id, user = u);
                        }

                        if (user.Role == null)
                        {
                            user.Role = new Role
                            {
                                Name = r.Name
                            };
                        }

                        return user;
                    },
                    param: parameters,
                    splitOn: "Role_Id"
                );

                await connection.CloseAsync();

                return lookup.Values.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> CountUsersAsync(Filter filter)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT COUNT(*) FROM \"User\" u " +
                    "JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\" WHERE 1=1 ");

                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(filter?.SearchQuery))
                {
                    stringBuilder.Append("AND (u.\"FirstName\" ILIKE @search OR u.\"LastName\" ILIKE @search OR u.\"Email\" ILIKE @search) ");
                    parameters.Add("@search", $"%{filter.SearchQuery}%");
                }
                if (filter?.IsApproved != null)
                {
                    stringBuilder.Append("AND u.\"IsApproved\" = @isApproved ");
                    parameters.Add("@isApproved", filter.IsApproved);
                }
                if (filter?.DoctorId != null)
                {
                    stringBuilder.Append("AND u.\"DoctorId\" = @doctorId ");
                    parameters.Add("@doctorId", filter.DoctorId);
                }

                await connection.OpenAsync();

                int count = await connection.ExecuteScalarAsync<int>(stringBuilder.ToString(), parameters);

                await connection.CloseAsync();

                return count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<User>> GetPatientsForDoctorAsync(Guid doctorId, Paging paging, Sorting sorting)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT u.*, r.\"Id\" as Role_Id, r.\"Name\" FROM \"User\" u " +
                    "JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\" " +
                    "WHERE u.\"IsActive\" = TRUE AND u.\"DoctorId\" = @doctorId ");

                var parameters = new DynamicParameters();
                parameters.Add("@doctorId", doctorId);
                stringBuilder.Append($" ORDER BY \"{sorting.OrderBy}\" {sorting.OrderDirection} " +
                    $"LIMIT @nextRows OFFSET @offset ");
                parameters.Add("@offset", paging.PageSize * (paging.PageNumber - 1));
                parameters.Add("@nextRows", paging.PageSize);

                await connection.OpenAsync();

                var lookup = new Dictionary<Guid, User>();

                await connection.QueryAsync<User, Role, User>(stringBuilder.ToString(),
                    (u, r) =>
                    {
                        if (!lookup.TryGetValue(u.Id, out var user))
                        {
                            lookup.Add(u.Id, user = u);
                        }

                        if (user.Role == null)
                        {
                            user.Role = new Role
                            {
                                Name = r.Name
                            };
                        }

                        return user;
                    },
                    parameters,
                    splitOn: "Role_Id"
                );

                await connection.CloseAsync();

                return lookup.Values.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "SELECT u.*, r.\"Name\" FROM \"User\" u " +
                    "JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\" " +
                    "WHERE u.\"IsActive\" = TRUE AND u.\"IsApproved\" = TRUE AND u.\"Email\" ILIKE @email";

                await connection.OpenAsync();

                var lookup = new Dictionary<Guid, User>();

                await connection.QueryAsync<User, Role, User>(commandText,
                    (u, r) =>
                    {
                        if (!lookup.TryGetValue(u.Id, out var user))
                        {
                            lookup.Add(u.Id, user = u);
                        }

                        if (user.Role == null)
                        {
                            user.Role = new Role
                            {
                                Name = r.Name
                            };
                        }

                        return user;
                    },
                    new { Email = email },
                    splitOn: "RoleId"
                );

                await connection.CloseAsync();

                return lookup.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "SELECT u.*, r.\"Id\" as Role_Id, r.\"Name\", e.\"Id\" AS Ethnicity_Id, e.\"Name\", e.\"Value\", el.\"Id\" AS EducationLevel_Id, " +
                        "el.\"Name\", el.\"Value\", du.\"Id\" AS Doctor_Id, du.\"FirstName\", du.\"LastName\" FROM \"User\" u " +
                    "LEFT JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\" " +
                    "LEFT JOIN \"Ethnicity\" e ON u.\"EthnicityId\" = e.\"Id\" " +
                    "LEFT JOIN \"EducationLevel\" el ON u.\"EducationLevelId\" = el.\"Id\" " +
                    "LEFT JOIN \"User\" du ON u.\"DoctorId\" = du.\"Id\" " +
                    "WHERE u.\"Id\" = @id";

                await connection.OpenAsync();

                var lookup = new Dictionary<Guid, User>();

                await connection.QueryAsync<User, Role, Ethnicity, EducationLevel, User, User>(commandText,
                    (u, r, e, el, du) =>
                    {
                        if (!lookup.TryGetValue(u.Id, out var user))
                        {
                            user = u;
                            user.Role = r;
                            user.Ethnicity = e;
                            user.EducationLevel = el;
                            user.Doctor = du;
                            lookup.Add(user.Id, user);
                        }

                        return user;
                    },
                    new { Id = id },
                    splitOn: "Role_Id,Ethnicity_Id,EducationLevel_Id,Doctor_Id"
                );

                await connection.CloseAsync();

                return lookup.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<User?> GetUserByOIBAsync(string oib)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "SELECT u.*, r.\"Id\" as Role_Id, r.\"Name\" FROM \"User\" u " +
                    "JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\" " +
                    "WHERE u.\"OIB\" = @oib AND u.\"IsActive\" = TRUE";

                await connection.OpenAsync();

                var lookup = new Dictionary<Guid, User>();

                await connection.QueryAsync<User, Role, User>(commandText,
                    (u, r) =>
                    {
                        if (!lookup.TryGetValue(u.Id, out var user))
                        {
                            lookup.Add(u.Id, user = u);
                        }

                        if (user.Role == null)
                        {
                            user.Role = new Role
                            {
                                Name = r.Name
                            };
                        }

                        return user;
                    },
                    new { OIB = oib },
                    splitOn: "Role_Id"
                );

                await connection.CloseAsync();

                return lookup.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Guid?> InsertUserAsync(User user)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "INSERT INTO \"User\" " +
                                        "(\"Id\", \"FirstName\", \"LastName\", \"OIB\", \"Email\", \"Password\", \"RoleId\", " +
                                        "\"IsApproved\", \"DateCreated\", \"DateUpdated\", \"CreatedByUserId\", " +
                                        "\"UpdatedByUserId\") " +
                                        "VALUES (@id, @firstName, @lastName, @oib, @email, @password, @roleId, " +
                                        "@isApproved, @dateCreated, @dateUpdated, @createdByUserId, @updatedByUserId) " +
                                        "RETURNING \"RoleId\";";

                await connection.OpenAsync();

                var result = await connection.QuerySingleAsync<Guid>(commandText, new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    oib = user.OIB,
                    email = user.Email,
                    password = user.Password,
                    roleId = user.Role.Id,
                    isApproved = user.IsApproved,
                    dateCreated = user.DateCreated,
                    dateUpdated = user.DateUpdated,
                    createdByUserId = user.CreatedByUserId,
                    updatedByUserId = user.UpdatedByUserId,
                });

                await connection.CloseAsync();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "UPDATE \"User\" SET" +
                    "\"FirstName\" = @firstName," +
                    "\"LastName\" = @lastName," +
                    "\"DateOfBirth\" = @dateOfBirth," +
                    "\"Gender\" = @gender," +
                    "\"EthnicityId\" = @ethnicityId," +
                    "\"EducationLevelId\" = @educationLevelId," +
                    "\"DoctorId\" = @doctorId," +
                    "\"IsActive\" = @isActive," +
                    "\"IsApproved\" = @isApproved," +
                    "\"DateUpdated\" = @dateUpdated," +
                    "\"UpdatedByUserId\" = @updatedByUserId " +
                    "WHERE \"Id\" = @id;";

                await connection.OpenAsync();

                var rowsAffected = await connection.ExecuteAsync(commandText, new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    dateOfBirth = user.DateOfBirth,
                    gender = user.Gender,
                    ethnicityId = user.EthnicityId,
                    educationLevelId = user.EducationLevelId,
                    doctorId = user.DoctorId,
                    isActive = user.IsActive,
                    isApproved = user.IsApproved,
                    dateUpdated = user.DateUpdated,
                    updatedByUserId = user.UpdatedByUserId
                });

                await connection.CloseAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
