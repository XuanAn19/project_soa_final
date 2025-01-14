using FinalProject.DTOs;
using FinalProject.Models;
using FinalProject.Repository.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FinalProject.Service.Interface
{
    public interface ITokenHandler
    {
        Task<(string, DateTime)> CreateAccessToken(User user);
        Task<(string, string, DateTime)> CreateRefreshToken(User user);
        Task Validate(TokenValidatedContext context);
        Task<TokenModels> ValidateRefreshToken(string refreshtoken);
    }
}
