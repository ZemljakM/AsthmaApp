using AsthmaApp.Models;

namespace AsthmaApp.Repository.Common
{
    public interface IEthnicityRepository
    {
        public Task<List<Ethnicity>> GetAllEthnicitiesAsync();
    }
}
