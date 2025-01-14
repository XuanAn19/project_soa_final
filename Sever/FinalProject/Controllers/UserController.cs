using FinalProject.Configuration;
using FinalProject.DTOs;
using FinalProject.Models;
using FinalProject.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        IUserService _userService;
        IRoleService _roleService;
        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }
        [HttpGet("infomation")]
        public async Task<IActionResult> GetUserByID()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null) return Unauthorized(new { status = "no", message = "Unauthorized" });
                var user = await _userService.GetUserByIdlAsync(userId);
                return Ok(new ApiResponse<UserDetailDTO> { Success = true, Message = "Ok" , Data = user});
                
            }
            catch(Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUserByID(string id)
        {
            try
            {
                var user = await _userService.GetUserByIdlAsync(id);
                return Ok(new ApiResponse<UserDetailDTO> { Success = true, Message = "Ok", Data = user });

            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        // Endpoint để cập nhật thông tin người dùng
        [HttpPut("update/{email}")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDTO updateUserDTO)
        {
            try
            {
                var userId = GetUserIdFromToken(); // Lấy userId từ thông tin người dùng đang đăng nhập
                if (userId == null) return Unauthorized(new { status = "no", message = "Unauthorized" });
                var updatedUser = await _userService.UpdateUserAsync(updateUserDTO, userId);
                return Ok(new { Message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(new { Success = true, Message = "Users fetched successfully", Data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("role/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] RoleUpdateDTO roleUpdateDTO)
        {
            try
            {
                await _userService.UpdateUserRoleAsync(id, roleUpdateDTO.RoleId);
                return Ok(new { Message = "User role updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(new { Message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        private string? GetUserIdFromToken()
        {
            var identityToken = HttpContext.User.Identity as ClaimsIdentity;
            return identityToken?.Claims.FirstOrDefault(c => c.Type == "IdUser")?.Value;
        }

    }
}
