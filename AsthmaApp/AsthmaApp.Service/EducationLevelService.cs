using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using AsthmaApp.Service.Common;

namespace AsthmaApp.Service
{
    public class EducationLevelService : IEducationLevelService
    {
        private IEducationLevelRepository _repository;

        public EducationLevelService(IEducationLevelRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EducationLevel>> GetAllEducationLevelsAsync()
        {
            return await _repository.GetAllEducationLevelsAsync();
        }
    }
}
