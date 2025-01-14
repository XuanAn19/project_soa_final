using FinalProject.DTOs;
using FinalProject.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountDTO accountDTO)
        {
            if(accountDTO == null)
            {
                return BadRequest("Lỗi");
            }
            var login = await _userService.Login(accountDTO);
            return Ok(login);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            try
            {
                if (register == null)
                {
                    return BadRequest("Lỗi");
                }
                await _userService.Register(register);
                return Ok(new { status = "ok", message = "Đăng kí thành công. Vui lòng kiếm tra email." , Data = register.Email });
            }
            catch(Exception ex)
            {
                return BadRequest(new { status = "no", message = ex.Message });
            }
        }

        [HttpPut("confirm")]
        public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmUserDTO confirm)
        {
            var confirmTrue = await _userService.ConfirmAccount(confirm);
            return confirmTrue;
        }


        //Tạo 1 method refreshtoken 
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO token)
        {
            if (token == null || string.IsNullOrEmpty(token.RefreshToken))
            {
                return BadRequest("Refresh token không hợp lệ.");
            }

            try
            {
                var tokenNew = await _userService.RefreshToken(token);
                return Ok(tokenNew);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra" +$": {ex.Message}");
            }
        }
        [HttpGet]
        [Authorize]
        public IActionResult ok() { return Ok("Okeee"); }

    }
}
