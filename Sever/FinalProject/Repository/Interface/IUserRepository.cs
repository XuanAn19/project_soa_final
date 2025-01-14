using FinalProject.Models;

namespace FinalProject.Repository.Interface
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdlAsync(string Id);
        Task Insert(User user);
        Task Update(User user);
        Task Delete(User user);
    }
}
