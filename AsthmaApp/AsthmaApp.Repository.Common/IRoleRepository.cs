using AsthmaApp.Models;

namespace AsthmaApp.Repository.Common
{
    public interface IRoleRepository
    {
        public Task<Role?> GetRoleByIdAsync(Guid id);

        public Task<Role?> GetRoleByNameAsync(string name);
    }
}
