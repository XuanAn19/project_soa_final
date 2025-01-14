using FinalProject.Data;
using FinalProject.Service.Interface;
using FinalProject.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;
using FinalProject.Repository.Interface;
using FinalProject.Repository;
using Newtonsoft.Json;
using FinalProject.Mail;
using FinalProject.Repository.UnitOfWork;

namespace FinalProject.Configuration
{
    public static class ConfigurationService
    {
        public static void RegisterContextDataBase(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<DataContext>(options => options.UseSqlServer(configuration.GetConnectionString("conn"),
                options => options.MigrationsAssembly(typeof(DataContext).Assembly.FullName)));
        }
        public static void RegisterDI(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped<IDbConnection>(db => new SqlConnection(configuration.GetConnectionString("conn")));
            service.AddScoped<IUserTokenRepository, UserTokenRepository>();
            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<IRoleRepository, RoleRepository>();
            service.AddScoped<IAddressRepository, AddressRepository>();

            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IRoleService, RoleService>();
            service.AddScoped<ITokenHandler, FinalProject.Service.TokenHandler>();
            service.AddScoped<IUserTokenService, UserTokenService>();

            service.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));


            service.AddTransient<ISendEmail, SendEmail>();
        }
        public static void AddTokenHandler(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>//[2]
                    {
                        var tokenHandler = context.HttpContext.RequestServices.GetRequiredService<ITokenHandler>();
                        return tokenHandler.Validate(context);
                    },
                    OnAuthenticationFailed = context =>//[3] Kh xác thực thất bại
                    {

                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<FinalProject.Service.TokenHandler>>();
                        logger.LogError("Authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>//1 
                    {
                        return Task.CompletedTask;
                    },
                    OnChallenge = context => //[4] phản hồi khi xác thực thất bại
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = new { message = "Bạn không có quyền truy cập." };
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                    },


                };
            });


        }

    }
}