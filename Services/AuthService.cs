using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LibraryApi.Data;
using LibraryApi.DTOs.Auth;
using LibraryApi.Models;
using LibraryApi.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.BearerToken;
using LibraryApi.Exceptions;

namespace LibraryApi.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _db;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration,
        RoleManager<IdentityRole> roleManager,
        AppDbContext db,
        ILogger<AuthService> logger
        )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _roleManager = roleManager;
        _db = db;
        _logger = logger;
    }

    public async Task CreateRoles()
    {
        string[] roles = { "Admin", "User" };

        foreach(var role in roles)
        {
            if(!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public async Task AssignAdminRole(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if(user == null)
        {
            return;
        }

        if(!await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
    }

    public async Task<IdentityResult> RegisterUser(RegisterUserDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };

        return await _userManager.CreateAsync(user, dto.Password);
    }

    public async Task<(string AccessToken, string RefreshToken)?> LoginUser(LoginUserDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);

        if(user == null)
        {
            throw new AppException(StatusCodes.Status404NotFound, "Wrong credentials");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            false
        );

        if(!result.Succeeded)
        {
            throw new AppException(StatusCodes.Status404NotFound, "Wrong credentials");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = _configuration["Jwt:Key"]!;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64)
        );

        var tokenHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken))
        );

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        _db.RefreshTokens.Add(refreshTokenEntity);

        await _db.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    public async Task LogoutUser(string refreshToken)
    {
        var tokenHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken))
        );

        var tokenEntity = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if(tokenEntity == null)
        {
            _logger.LogInformation("Logout attempted with an invalid or non-existent refresh token.");
            return;
        }

        tokenEntity.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task <(string AccessToken, string RefreshToken)?> RefreshToken(string refreshToken)
    {
        var tokenHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken))
        );
    
        var currentToken = await _db.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if(currentToken == null || currentToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new AppException(StatusCodes.Status401Unauthorized, "Invalid token");
        }

        if(currentToken.RevokedAt != null)
        {
            var userTokens = _db.RefreshTokens.Where(rt => rt.UserId == currentToken.UserId && rt.RevokedAt == null);

            foreach(var t in userTokens)
            {
                t.RevokedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return null;
        }


        currentToken.RevokedAt = DateTime.UtcNow;

        var user = currentToken.User;
        var roles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = _configuration["Jwt:Key"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var newRefreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(newRefreshToken)));

        _db.RefreshTokens.Add(new RefreshToken
        {
           TokenHash = newRefreshTokenHash,
           ExpiresAt = DateTime.UtcNow.AddDays(7),
           UserId = user.Id 
        });

        await _db.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }
}