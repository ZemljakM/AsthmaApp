using AsthmaApp.Models;

namespace AsthmaApp.Repository.Common
{
    public interface IEducationLevelRepository
    {
        public Task<List<EducationLevel>> GetAllEducationLevelsAsync();
    }
}
