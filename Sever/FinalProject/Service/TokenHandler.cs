using FinalProject.DTOs;
using FinalProject.Models;
using FinalProject.Models.Token;
using FinalProject.Repository.Interface;
using FinalProject.Repository.UnitOfWork;
using FinalProject.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalProject.Service
{
    public class TokenHandler : ITokenHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IUserTokenService _userTokenService;
        private readonly IUnitOfWork _unitOfWork;
        public TokenHandler(IConfiguration configuration, IUserTokenService userTokenService, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _userTokenService = userTokenService;
            _unitOfWork = unitOfWork;
        }
        public async Task<(string, DateTime)> CreateAccessToken(User user)
        {
            DateTime exp = DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:timeAccessTokenByMinutes"]));
            var roleName = await _unitOfWork.roleRepository.GetRoleByIdAsync(user.RoleId);
            var roleClaim = new Claim(ClaimTypes.Role, roleName.roleName);
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String, _configuration["Jwt:Issuer"]),
                // Claim này sử dụng `Jti` (JWT ID) là một mã định danh duy nhất cho mỗi token, giúp ngăn chặn việc tái sử dụng token cũ.
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"],ClaimValueTypes.String, _configuration["Jwt:Issuer"]),
                // new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToString(),ClaimValueTypes.Integer64, _configuration["Jwt:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration["Jwt:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Exp, exp.ToString("yyyy/MM/dd hh:mm:ss"), ClaimValueTypes.Integer64, _configuration["Jwt:Issuer"]),

                  new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"],ClaimValueTypes.Integer64, _configuration["Jwt:Issuer"]),
                  // new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddHours(3).ToString("yyyy/MM/dd hh:mm:ss"),ClaimValueTypes.Integer64,_configuration["Jwt:Issuer"]),
                // Claim `Name` thông tin người dùng
                new Claim(ClaimTypes.NameIdentifier , user.Id.ToString(),ClaimValueTypes.String,_configuration["Jwt:Issuer"]),
                new Claim(ClaimTypes.Name , user.FullName,ClaimValueTypes.String,_configuration["Jwt:Issuer"]),
                    new Claim("Email" , user.Email,ClaimValueTypes.String,_configuration["Jwt:Issuer"]),
                     new Claim("IdUser" , user.Id,ClaimValueTypes.String,_configuration["Jwt:Issuer"]),
                     roleClaim,

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenInfo = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: exp,
                credential

                );
            string tokenAccess = new JwtSecurityTokenHandler().WriteToken(tokenInfo);
            return await Task.FromResult((tokenAccess, exp));
        }


        public async Task<(string, string, DateTime)> CreateRefreshToken(User user)
        {
            DateTime expRefresh = DateTime.Now.AddHours(int.Parse(_configuration["Jwt:timeRefreshTokenByHours"]));
            string code = Guid.NewGuid().ToString();
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String, _configuration["Jwt:Issuer"]),
                // Claim này sử dụng `Jti` (JWT ID) là một mã định danh duy nhất cho mỗi token, giúp ngăn chặn việc tái sử dụng token cũ.
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"],ClaimValueTypes.String, _configuration["Jwt:Issuer"]),
                // new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToString(),ClaimValueTypes.Integer64, _configuration["Jwt:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration["Jwt:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Exp, expRefresh.ToString("yyyy/MM/dd hh:mm:ss"), ClaimValueTypes.Integer64, _configuration["Jwt:Issuer"]),
                new Claim(ClaimTypes.SerialNumber, code, ClaimValueTypes.String, _configuration["Jwt:Issuer"])

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenInfo = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: expRefresh,
                credential

                );
            string tokenRefresh = new JwtSecurityTokenHandler().WriteToken(tokenInfo);
            return await Task.FromResult((code, tokenRefresh, expRefresh));
        }

        //Validate RefreshToken : Tạo 1 cái model khi clien chuyền cái RefreshToken lên ta viết 1 cái method  để validate RefreshToken này,
        // chúng ta sẽ lấy SerialNumber của RefreshToken ra và chúng ta get được cái code(codeRefreshToken) tong database chúng ta check thêm lần nữa.Viết phương thức
        public async Task<TokenModels> ValidateRefreshToken(string refreshtoken)
        {
            TokenModels tokenModels = new TokenModels();
            //Gọi method TokenModels và dungf refreshtoken để validate
            var claimPriciple = new JwtSecurityTokenHandler().ValidateToken(refreshtoken
                , new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                },
                    out _ //Không cần khởi tạo, là một biến bỏ qua trong C#. Nó nhận giá trị của SecurityToken được tạo bởi phương thức ValidateToken()
                );
            if (claimPriciple == null) return tokenModels;

            string code = claimPriciple.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            if (string.IsNullOrEmpty(code)) return tokenModels;

            UserToken userToken = await _userTokenService.CheckRefreshToken(code);

            if (userToken != null)
            {
                User user = await _unitOfWork.userRepository.GetUserByIdlAsync(userToken.UserId);

                (string newtokenAccess, DateTime exp) = await CreateAccessToken(user);
                (string codeRefreshToken, string newtokenRefresh, DateTime newexpRefresh) = await CreateRefreshToken(user);

                // Trả về token trong JSON
                return new TokenModels
                {
                    AccessToken = newtokenAccess,
                    RefreshToken = newtokenRefresh,
                    Fullname = user.FullName,
                    Email = user.Email
                };
            }
            return tokenModels;


        }

        public async Task Validate(TokenValidatedContext context)
        {
            var claims = context.Principal.Claims.ToList();

            if (claims.Count == 0)
            {
                context.Fail("No token information.");
                return;
            }
            var identity = context.Principal.Identity as ClaimsIdentity;
            if (identity.FindFirst(JwtRegisteredClaimNames.Iss) == null)
            {
                context.Fail("The token does not match the issuer organization.");
                return;
            }
            if (identity.FindFirst("Email") != null)
            {
                string email = identity.FindFirst("Email").Value;
                var user = await _unitOfWork.userRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    context.Fail("Email not found");
                    return;
                }
            }

        }
    }
}
