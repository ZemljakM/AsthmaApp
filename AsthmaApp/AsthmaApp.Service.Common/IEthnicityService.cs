using AsthmaApp.Models;

namespace AsthmaApp.Service.Common
{
    public interface IEthnicityService
    {
        public Task<List<Ethnicity>> GetAllEthnicitiesAsync();
    }
}
