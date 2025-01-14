using FinalProject.DTOs;
using FinalProject.Models;
using FinalProject.Models.Token;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Service.Interface
{
    public interface IUserService
    {
        Task<object> Login(AccountDTO account);
        Task Register(RegisterDTO register);
        Task<TokenModels> RefreshToken(RefreshTokenDTO refreshToken);
        Task<User> GetUserByEmailAsync(string email);
        Task<IActionResult> ConfirmAccount(ConfirmUserDTO confirm);
        Task<User> UpdateUserAsync(UpdateUserDTO user, string email);
        Task<UserDetailDTO> GetUserByIdlAsync(string id);
        Task<List<UserDetailDTO>> GetAllUsersAsync();
        Task UpdateUserRoleAsync(string userId, string roleId);
        Task DeleteUserAsync(string userId);
    }
}
