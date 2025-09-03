using AsthmaApp.Common;
using AsthmaApp.Models;

namespace AsthmaApp.Service.Common
{
    public interface IUserService
    {
        public Task<User?> AuthenticateAsync(string email, string password);

        public Task<User> CreateUserAsync();

        public Task<bool> DeleteUserAsync(Guid id);
        public Task<bool> DeactivateUserAsync(Guid id);

        public Task<List<User>> GetAllUsersAsync(Filter filter, Paging paging, Sorting sorting);

        public Task<List<User>> GetPatientsForDoctorAsync(Guid doctorId, Paging paging, Sorting sorting);

        public Task<User?> GetUserByEmailAsync(string email);

        public Task<User> GetUserByIdAsync(Guid id);
        
        public Task<User?> GetUserByOIBAsync(string oib);

        public Task<User> InitializeAsync(User user);

        public Task<int> CountUsersAsync(Filter filter);

        public Task<Guid?> InsertUserAsync(User user);

        public Task<bool> IsProfileCompletedAsync(Guid id);

        public Task<bool> UpdateUserAsync(User user);
    }
}
