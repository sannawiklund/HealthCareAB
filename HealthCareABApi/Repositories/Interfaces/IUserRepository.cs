using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string id);
        Task<User>GetByUsernameAsync(string username);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(string id, User user);
        Task DeleteAsync(string id);
    }
}
