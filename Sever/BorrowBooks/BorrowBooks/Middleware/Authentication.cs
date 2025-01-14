﻿using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BorrowBooks.Middleware
{
    public class Authentication
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly string _allowedPort = "7777";  // Port của API Gateway

        public Authentication(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                /*var requestPort = context.Request.Host.Port;

                if (requestPort.HasValue && requestPort.Value != int.Parse(_allowedPort))
                {
                    // Nếu yêu cầu không đến từ cổng 5555, trả về Forbidden
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Access Denied: Invalid port.");
                    return;
                }
                else
                {
                   
                }*/
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token != null)
                {
                    var auth = AttachUserToContext(context, token);
                    if (auth)
                    {
                        await _next(context);
                    }
                    else
                    {
                        var myObject = new { status = "no", message = "Token không đúng" };
                        var jsonResponse = JsonConvert.SerializeObject(myObject);
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync(jsonResponse);
                    }
                }
                else
                {
                    var myObject = new { status = "no", message = "Unauthorized: Missing or invalid token." };
                    var jsonResponse = JsonConvert.SerializeObject(myObject);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                var myObject = new { status = "no", message = ex.Message };
                var jsonResponse = JsonConvert.SerializeObject(myObject);
                await context.Response.WriteAsync(jsonResponse);
            }
        }

        private Boolean AttachUserToContext(HttpContext context, string token)
        {
            // Thực hiện giải mã token và lưu thông tin vào HttpContext
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            try
            {
                var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "IdUser").Value;
                var email = jwtToken.Claims.First(x => x.Type == "Email").Value;
                var fullName = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                var role = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                // Đính kèm thông tin người dùng vào context
                context.Items["UserId"] = userId;
                context.Items["Email"] = email;
                context.Items["FullName"] = fullName;
                context.Items["role"] = role;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
