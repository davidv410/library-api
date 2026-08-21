using LibraryApi.Data;
using LibraryApi.DTOs.Auth;
using LibraryApi.Models;
using LibraryApi.Services;
using Microsoft.AspNetCore.Identity;

namespace LibraryApi.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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
}