using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using AsthmaApp.Service.Common;

namespace AsthmaApp.Service
{
    public class EthnicityService : IEthnicityService
    {
        private IEthnicityRepository _repository;

        public EthnicityService(IEthnicityRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Ethnicity>> GetAllEthnicitiesAsync()
        {
            return await _repository.GetAllEthnicitiesAsync();
        }
    }
}
