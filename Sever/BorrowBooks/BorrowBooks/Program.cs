using BorrowBooks.DTOs;
using BorrowBooks.Middleware;
using BorrowBooks.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Connect Db
builder.Services.RegisterContextDataBase(builder.Configuration);
builder.Services.RegisterDI(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() // Cho phép tất cả các origin
                   .AllowAnyMethod() // Cho phép tất cả các phương thức (GET, POST, PUT, DELETE, v.v.)
                   .AllowAnyHeader(); // Cho phép tất cả các headers
        });
});
// Add services to the container.
builder.Services.AddHttpClient("BookService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7135"); // Gọi thông qua API Gateway
    client.Timeout = TimeSpan.FromSeconds(90);
});
builder.Services.AddHttpClient("UserService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7000"); // Gọi thông qua API Gateway
    client.Timeout = TimeSpan.FromSeconds(90);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<Authentication>();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
