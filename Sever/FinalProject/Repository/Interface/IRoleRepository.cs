using FinalProject.Models;

namespace FinalProject.Repository.Interface
{
    public interface IRoleRepository
    {
        Task AddRoleAsync(Role role);
        Task<Role> GetRoleByNameAsync(string name);
        Task<List<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(string name);
        Task UpdateRoleAsync(Role role);
    }
}
