using AsthmaApp.Models;

namespace AsthmaApp.Service.Common
{
    public interface IRoleService
    {
        public Task<Role?> GetRoleByIdAsync(Guid id);

        public Task<Role?> GetRoleByNameAsync(string name);
    }
}
