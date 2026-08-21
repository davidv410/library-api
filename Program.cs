using LibraryApi.Data;
using LibraryApi.Exceptions;
using LibraryApi.Services;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
       ValidateIssuerSigningKey = true,
       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),

       ValidateIssuer = true,
       ValidIssuer = builder.Configuration["Jwt:Issuer"],

       ValidateAudience = true,
       ValidAudience = builder.Configuration["Jwt:Audience"],

       ValidateLifetime = true
    };
});

builder.Services.AddControllers();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>().AddSignInManager().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DatabaseURL")
    )
);

builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

    await authService.CreateRoles();
    await authService.AssignAdminRole("David");
}

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();