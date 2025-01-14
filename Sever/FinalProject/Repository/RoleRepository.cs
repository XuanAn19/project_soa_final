using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Repository
{
    public class RoleRepository : IRoleRepository
    {
        DataContext _dataContext;
        public RoleRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Role> GetRoleByNameAsync(string name)
        {
            return await _dataContext.Roles.FirstOrDefaultAsync(n => n.roleName == name);
        }
        public async Task AddRoleAsync(Role role)
        {
            await _dataContext.Roles.AddAsync(role);
        }

        public async Task DeleteRoleAsync(Role role)
        {
            _dataContext.Roles.Remove(role);
        }

        // Cập nhật quyền
        public async Task UpdateRoleAsync(Role role)
        {
            _dataContext.Roles.Update(role);
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _dataContext.Roles.ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(string Id)
        {
            return await _dataContext.Roles.FirstOrDefaultAsync(x => x.Id == Id);
        }

    }
}
