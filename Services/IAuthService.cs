using Microsoft.AspNetCore.Identity;
using LibraryApi.DTOs.Auth;

namespace LibraryApi.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterUser(RegisterUserDto dto);
    Task<string?> LoginUser(LoginUserDto dto);
    Task CreateRoles();
    Task AssignAdminRole(string username);
}