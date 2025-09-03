using AsthmaApp.Common;
using AsthmaApp.Models;

namespace AsthmaApp.Repository.Common
{
    public interface IRecordRepository 
    {
        public Task<List<Record>> GetAllRecordsAsync(Filter filter, Paging paging, Sorting sorting);
        public Task<Record?> GetRecordByIdAsync(Guid id);
        public Task<bool> InsertRecordAsync(Record record);
        public Task<int> CountRecordsAsync(Filter filter);
        public Task<bool> UpdateRecordAsync(Record record);
    }
}
