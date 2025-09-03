using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using AsthmaApp.Service.Common;

namespace AsthmaApp.Service
{
    public class RoleService: IRoleService
    {
        private IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Role?> GetRoleByIdAsync(Guid id)
        {
            return await _repository.GetRoleByIdAsync(id);
        }

        public async Task<Role?> GetRoleByNameAsync(string name)
        {
            return await _repository.GetRoleByNameAsync(name);
        }
    }
}
