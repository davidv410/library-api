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

namespace LibraryApi.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly AppDbContext _db;

    public AuthService(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration,
        RoleManager<IdentityRole> roleManager,
        AppDbContext db
        )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _roleManager = roleManager;
        _db = db;
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

    public async Task<string?> LoginUser(LoginUserDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);

        if(user == null)
        {
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            false
        );

        if(!result.Succeeded)
        {
            return null;
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

        return accessToken;
    }
}