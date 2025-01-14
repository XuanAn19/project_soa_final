using BorrowBooks.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

namespace BorrowBooks.DTOs
{
    public static class ConfigurationService
    {
        public static void RegisterContextDataBase(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<BorrowDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("BookApiConnectString"),
                options => options.MigrationsAssembly(typeof(BorrowDbContext).Assembly.FullName)));
        }
        public static void RegisterDI(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = false,                // Kiểm tra Issuer
                     ValidIssuer = "https://localhost:7228", // Phải khớp với _config["Jwt:Issuer"] trong SOA

                     ValidateAudience = false,              // Kiểm tra Audience
                     ValidAudience = "http://localhost:4200",       // Phải khớp với _config["Jwt:Audience"] trong SOA

                     ValidateLifetime = true,              // Kiểm tra thời gian sốngS của token
                     ClockSkew = TimeSpan.Zero,            // Không cho phép trễ thời gian

                     ValidateIssuerSigningKey = true,      // Kiểm tra chữ ký của token
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes("a3e6b875-cc82-4e4d-aad7-96d887ab2197") // Key từ SOA
                     )
                 };
             });

            service.AddScoped<IDbConnection>(db => new SqlConnection(configuration.GetConnectionString("BookApiConnectString")));

        }

    }
}
