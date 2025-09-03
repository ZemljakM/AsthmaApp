using AsthmaApp.Common;
using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Text;

namespace AsthmaApp.Repository
{
    public class RecordRepository : IRecordRepository
    {
        private readonly string _connectionString;

        public RecordRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Record>> GetAllRecordsAsync(Filter filter, Paging paging, Sorting sorting)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT r.* FROM \"Record\" r WHERE 1=1 ");

                var parameters = new DynamicParameters();

                if (filter?.UserId.HasValue == true && filter?.UserId != Guid.Empty)
                {
                    stringBuilder.Append("AND r.\"UserId\" = @userId ");
                    parameters.Add("@userId",filter.UserId);
                }

                stringBuilder.Append($" ORDER BY \"{sorting.OrderBy}\" {sorting.OrderDirection} " +
                    $"LIMIT @nextRows OFFSET @offset ");
                parameters.Add("@offset", paging.PageSize * (paging.PageNumber - 1));
                parameters.Add("@nextRows", paging.PageSize);

                await connection.OpenAsync();

                var records = await connection.QueryAsync<Record>(stringBuilder.ToString(), parameters);

                await connection.CloseAsync();

                return records.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Record?> GetRecordByIdAsync(Guid id)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "SELECT r.*, l.\"Id\" AS Location_Id, l.\"City\", l.\"Country\" FROM \"Record\" r " +
                    "LEFT JOIN \"Location\" l ON r.\"LocationId\" = l.\"Id\" " +
                    "WHERE r.\"IsActive\" = TRUE AND r.\"Id\" = @id";

                await connection.OpenAsync();

                var lookup = new Dictionary<Guid, Record>();

                await connection.QueryAsync<Record, Location, Record>(commandText,
                    (r, l) =>
                    {
                        if (!lookup.TryGetValue(r.Id, out var record))
                        {
                            lookup.Add(r.Id, record = r);
                        }

                        if (record.Location == null)
                        {
                            record.Location = new Location
                            {
                                City = l.City,
                                Country = l.Country,
                            };
                        }

                        return record;
                    },
                    new { Id = id},
                    splitOn: "Location_Id"
                );

                await connection.CloseAsync();

                return lookup.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<bool> InsertRecordAsync(Record record)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "INSERT INTO \"Record\" " +
                                        "(\"Id\", \"BMI\", \"Smoking\", \"PhysicalActivity\", \"DietQuality\", \"SleepQuality\", \"PollenExposure\", \"PollutionExposure\", " +
                                        "\"DustExposure\", \"PetAllergy\", \"FamilyHistoryOfAsthma\", \"HistoryOfAllergies\", \"Eczema\", \"HayFever\", " +
                                        "\"GastroesophagealReflux\", \"Wheezing\", \"ShortnessOfBreath\", \"ChestTightness\", \"Coughing\", \"NighttimeSymptoms\", " +
                                        "\"ExerciseInduced\", \"LocationId\", \"Diagnosis\", \"IsApproved\", \"UserId\", \"Recommendation\", \"IsActive\", \"DateCreated\", " +
                                        "\"DateUpdated\", \"CreatedByUserId\", \"UpdatedByUserId\") " +
                                        "VALUES (@id, @bmi, @smoking, @physicalactivity, @dietQuality, @sleepQuality, @pollenExposure, @pollutionExposure, @dustExposure, @petAllergy, " +
                                        "@familyHistoryOfAsthma, @historyOfAllergies, @eczema, @hayFever, @gastroesophagealReflux, @wheezing, @shortnessOfBreath, @chestTightness, @coughing, " +
                                        "@nighttimeSymptoms, @exerciseInduced, @locationId, @diagnosis, @isApproved, @userId, @recommendation, @isActive, @dateCreated, @dateUpdated, @createdByUserId, @updatedByUserId);";

                await connection.OpenAsync();

                var rowsAffected = await connection.ExecuteAsync(commandText, new
                {
                    id = record.Id,
                    bmi = record.BMI,
                    smoking = record.Smoking,
                    physicalActivity = record.PhysicalActivity,
                    dietQuality = record.DietQuality,
                    sleepQuality = record.SleepQuality,
                    pollenExposure = record.PollenExposure,
                    pollutionExposure = record.PollutionExposure,
                    dustExposure = record.DustExposure,
                    petAllergy = record.PetAllergy,
                    familyHistoryOfAsthma = record.FamilyHistoryOfAsthma,
                    historyOfAllergies = record.HistoryOfAllergies,
                    eczema = record.Eczema,
                    hayFever = record.HayFever,
                    gastroesophagealReflux = record.GastroesophagealReflux,
                    wheezing = record.Wheezing,
                    shortnessOfBreath = record.ShortnessOfBreath,
                    chestTightness = record.ChestTightness,
                    coughing = record.Coughing,
                    nighttimeSymptoms = record.NighttimeSymptoms,
                    exerciseInduced = record.ExerciseInduced,
                    locationId = record.LocationId,
                    diagnosis = record.Diagnosis,
                    isApproved = record.IsApproved,
                    userId = record.UserId,
                    recommendation = record.Recommendation,
                    isActive = record.IsActive,
                    dateCreated = record.DateCreated,
                    dateUpdated = record.DateUpdated,
                    createdByUserId = record.CreatedByUserId,
                    updatedByUserId = record.UpdatedByUserId
                });

                return rowsAffected > 0;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

        public async Task<int> CountRecordsAsync(Filter filter)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT COUNT(*) FROM \"Record\" r WHERE 1=1 ");

                var parameters = new DynamicParameters();

                if (filter?.UserId.HasValue == true && filter?.UserId != Guid.Empty)
                {
                    stringBuilder.Append("AND r.\"UserId\" = @userId ");
                    parameters.Add("@userId", filter.UserId);
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

        public async Task<bool> UpdateRecordAsync(Record record)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string commandText = "UPDATE \"Record\" SET" +
                    "\"Recommendation\" = @recommendation," +
                    "\"IsApproved\" = @isApproved," +
                    "\"FVC\" = @fvc," +
                    "\"FEV1\" = @fev1 " +
                    "WHERE \"Id\" = @id;";

                await connection.OpenAsync();

                var rowsAffected = await connection.ExecuteAsync(commandText, new
                {
                    id = record.Id,
                    recommendation = record.Recommendation,
                    isApproved = record.IsApproved,
                    fvc = record.FVC,
                    fev1 = record.FEV1
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
