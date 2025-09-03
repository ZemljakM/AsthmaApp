using AsthmaApp.Models;

namespace AsthmaApp.Service.Common
{
    public interface ILocationService
    {
        public Task<Location?> GetCoordinatesAsync(string city, string country);
        public Task<Location?> GetLocationAsync(string city, string country);
        public Task<bool> InsertLocationAsync(Location location);
    }
}
