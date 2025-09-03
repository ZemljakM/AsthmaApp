using AsthmaApp.Common;
using AsthmaApp.Models;

namespace AsthmaApp.Repository.Common
{
    public interface IUserRepository 
    {
        public Task<bool> DeleteUserAsync(Guid id);

        public Task<bool> DeactivateUserAsync(User user);

        public Task<List<User>> GetAllUsersAsync(Filter filter, Paging paging, Sorting sorting);

        public Task<List<User>> GetPatientsForDoctorAsync(Guid doctorId, Paging paging, Sorting sorting);

        public Task<User?> GetUserByEmailAsync(string email);

        public Task<User> GetUserByIdAsync(Guid id);
        
        public Task<User?> GetUserByOIBAsync(string oib);

        public Task<Guid?> InsertUserAsync(User user);

        public Task<bool> UpdateUserAsync(User user);

        public Task<int> CountUsersAsync(Filter filter);
    }
}
