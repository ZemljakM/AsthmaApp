using AsthmaApp.Common;
using AsthmaApp.Models;

namespace AsthmaApp.Service.Common
{
    public interface IRecordService
    {
        public Task<Record> CreateRecordAsync();

        public Task<List<Record>> GetAllRecordsAsync(Filter filter, Paging paging, Sorting sorting);

        public Task<Record?> GetRecordByIdAsync(Guid id);

        public Task<string> GeneratePromptAsync(Record record, User user);

        public Task<double?> GetPollenExposureAsync(double latitude, double longitude);

        public Task<double?> GetPollutionExposureAsync(double latitude, double longitude);

        public Task<bool> InsertRecordAsync(Record record);

        public Task<double?> PredictAsthmaAsync(Record record, User userProfile);

        public Task<int> CountRecordsAsync(Filter filter);

        public Task<bool> UpdateRecordAsync(Record record);
    }
}
