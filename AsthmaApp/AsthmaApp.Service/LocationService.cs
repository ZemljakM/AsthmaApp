using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using AsthmaApp.Service.Common;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace AsthmaApp.Service
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _googleApiKey;
        private ILocationRepository _repository;

        public LocationService(HttpClient httpClient, IConfiguration config, ILocationRepository repository)
        {
            _httpClient = httpClient;
            _googleApiKey = config["GoogleMaps:ApiKey"];
            _repository = repository;
        }

        public async Task<Location> CreateLocationAsync()
        {
            var location = new Location();
            location.Id = Guid.NewGuid();
            location.IsActive = true;
            location.DateCreated = DateTime.Now;
            location.DateUpdated = DateTime.Now;
            return location;
        }

        public async Task<Location?> GetCoordinatesAsync(string city, string country)
        {
            var address = $"{city},{country}";
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_googleApiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var location = json.RootElement
                .GetProperty("results")[0]
                .GetProperty("geometry")
                .GetProperty("location");

            var entity = await CreateLocationAsync();
            entity.Latitude = location.GetProperty("lat").GetDouble();
            entity.Longitude = location.GetProperty("lng").GetDouble();
            entity.City = city;
            entity.Country = country;

            var result = await InsertLocationAsync(entity);

            return result ? entity : null;
        }

        public async Task<Location?> GetLocationAsync(string city, string country)
        {
            return await _repository.GetLocationAsync(city, country);
        }

        public async Task<bool> InsertLocationAsync(Location location)
        {
            return await _repository.InsertLocationAsync(location);
        }
    }
}
