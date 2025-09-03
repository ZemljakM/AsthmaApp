using AsthmaApp.Models;

namespace AsthmaApp.Service.Common
{
    public interface IEducationLevelService
    {
        public Task<List<EducationLevel>> GetAllEducationLevelsAsync();
    }
}
