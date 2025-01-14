using FinalProject.DTOs;
using FinalProject.Models;
using FinalProject.Repository.Interface;
using FinalProject.Repository.UnitOfWork;
using FinalProject.Service.Interface;

namespace FinalProject.Service
{
    public class RoleService : IRoleService
    {
        IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateRole(RoleDTO role)
        {
            var roles = new Models.Role
            {
                roleName = role.Name,
                description = role.Description,
            };
            await _unitOfWork.roleRepository.AddRoleAsync(roles);
            await _unitOfWork.SaveChangeAsync();
        }
        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _unitOfWork.roleRepository.GetAllRolesAsync();
        }

        public async Task<Role> GetRolesByIdAsync(string roleId)
        {
            return await _unitOfWork.roleRepository.GetRoleByIdAsync(roleId);
        }

        // Cập nhật quyền
        public async Task UpdateRole(string id, RoleDTO roleDTO)
        {
            var role = await _unitOfWork.roleRepository.GetRoleByIdAsync(id);
            if (role == null)
            {
                throw new Exception("Role not found");
            }

            role.roleName = roleDTO.Name;
            await _unitOfWork.roleRepository.UpdateRoleAsync(role);
            await _unitOfWork.SaveChangeAsync();
        }
        // Xóa quyền
        public async Task DeleteRole(string id)
        {
            var role = await _unitOfWork.roleRepository.GetRoleByIdAsync(id);
            if (role == null)
            {
                throw new Exception("Role not found");
            }

            await _unitOfWork.roleRepository.DeleteRoleAsync(role);
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
