using AsthmaApp.Models;

namespace AsthmaApp.Repository.Common
{
    public interface ILocationRepository
    {
        public Task<Location?> GetLocationAsync(string city, string country);
        public Task<bool> InsertLocationAsync(Location location);
    }
}
