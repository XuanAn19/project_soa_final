using FinalProject.DTOs;
using FinalProject.Models;

namespace FinalProject.Service.Interface
{
    public interface IRoleService
    {
        Task CreateRole(RoleDTO role);
        Task DeleteRole(string id);
        Task<List<Role>> GetAllRolesAsync();
        Task<Role> GetRolesByIdAsync(string roleId);
        Task UpdateRole(string id, RoleDTO roleDTO);
    }
}
