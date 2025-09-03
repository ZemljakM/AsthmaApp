using AsthmaApp.Models;

namespace AsthmaApp.Service.Common
{
    public interface IAuthService
    {
        public Task<string> GenerateTokenAsync(User user);
    }
}
