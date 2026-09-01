using LibraryApi.Data;
using LibraryApi.Exceptions;
using LibraryApi.Services;
using LibraryApi.Models;
using LibraryApi.Hubs;
using Microsoft.AspNetCore.SignalR; 
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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddControllers();

builder.Services.AddSignalR();

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

builder.Services.AddScoped<IBookReviewService, BookReviewService>();

builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();

builder.Services.AddScoped<IBookNotificationService, BookNotificationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCors", policy =>
    {
        policy
        .WithOrigins("http://127.0.0.1:5500")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

    await authService.CreateRoles();
    await authService.AssignAdminRole("David");
}

app.UseCors("SignalRCors");

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();
app.MapHub<MessagingHub>("/hubs/messaging");

app.Run();