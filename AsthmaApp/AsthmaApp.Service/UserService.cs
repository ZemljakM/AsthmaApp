using AsthmaApp.Common;
using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using AsthmaApp.Service.Common;

namespace AsthmaApp.Service
{
    public class UserService : IUserService
    {
        private IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _repository.GetUserByEmailAsync(email);

            if ( user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }

            return null;
        }

        public async Task<int> CountUsersAsync(Filter filter)
        {
            return await _repository.CountUsersAsync(filter);
        }

        public async Task<User> CreateUserAsync()
        {
            var user = new User();
            user.Id = Guid.NewGuid();
            user.DateCreated = DateTime.Now;
            user.DateUpdated = DateTime.Now;
            user.CreatedByUserId = user.Id;
            user.UpdatedByUserId = user.Id;
            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            return await _repository.DeleteUserAsync(id);
        }

        public async Task<bool> DeactivateUserAsync(Guid id)
        {
            var user = await _repository.GetUserByIdAsync(id);

            if (user is null)
                return false;

            user = await InitializeAsync(user);

            return await _repository.DeactivateUserAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync(Filter filter, Paging paging, Sorting sorting)
        {
            return await _repository.GetAllUsersAsync(filter, paging, sorting);
        }

        public async Task<List<User>> GetPatientsForDoctorAsync(Guid doctorId, Paging paging, Sorting sorting)
        {
            return await _repository.GetPatientsForDoctorAsync(doctorId, paging, sorting);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _repository.GetUserByEmailAsync(email);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByOIBAsync(string oib)
        {
            return await _repository.GetUserByOIBAsync(oib);
        }

        public async Task<User> InitializeAsync(User user)
        {
            user.DateUpdated = DateTime.Now;
            user.UpdatedByUserId = user.Id;
            return user;
        }

        public async Task<Guid?> InsertUserAsync(User user)
        {
            return await _repository.InsertUserAsync(user);
        }

        public async Task<bool> IsProfileCompletedAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);

            if (user == null)
                return false;

            return !string.IsNullOrEmpty(user.Gender) && user.EthnicityId.HasValue && user.EducationLevelId.HasValue && user.DateOfBirth.HasValue;

        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            user = await InitializeAsync(user);
            return await _repository.UpdateUserAsync(user);
        }
    }
}
