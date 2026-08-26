using Microsoft.AspNetCore.Identity;
using LibraryApi.DTOs.Auth;

namespace LibraryApi.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterUser(RegisterUserDto dto);
    Task<(string AccessToken, string RefreshToken)?> LoginUser(LoginUserDto dto);
    Task CreateRoles();
    Task AssignAdminRole(string username);
    Task LogoutUser(string refreshToken);
    Task <(string AccessToken, string RefreshToken)?> RefreshToken(string refreshToken);
}