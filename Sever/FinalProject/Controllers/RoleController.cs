using FinalProject.DTOs;
using FinalProject.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/v1/roles")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO roleDTO)
        {
            try
            {
                await _roleService.CreateRole(roleDTO);
                return Ok(new { Message = "Create role successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(new { Success = true, Data = roles });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetRolesById(string id)
        {
            try
            {
                var roles = await _roleService.GetRolesByIdAsync(id);
                return Ok(new { Success = true, Data = roles });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleDTO roleDTO)
        {
            try
            {
                await _roleService.UpdateRole(id, roleDTO);
                return Ok(new { Message = "Role updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                await _roleService.DeleteRole(id);
                return Ok(new { Message = "Role deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

    }
}
