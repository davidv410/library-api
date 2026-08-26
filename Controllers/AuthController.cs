using LibraryApi.DTOs.Auth;
using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var result = await _authService.RegisterUser(dto);

        if(!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var result = await _authService.LoginUser(dto);

        if(result == null)
        {
            return Unauthorized();
        }

        Response.Cookies.Append(
            "refreshToken",
            result.Value.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            }
        );

        return Ok(new
        {
            accesToken = result.Value.AccessToken
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if(refreshToken == null)
        {
            return Ok();
        }

        await _authService.LogoutUser(refreshToken);
        Response.Cookies.Delete("refreshToken");
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if(refreshToken == null)
        {
            return Unauthorized();
        }

        var result = await _authService.RefreshToken(refreshToken); 

        if(result == null)
        {
            Response.Cookies.Delete("refreshToken");
            return Unauthorized();
        }

        Response.Cookies.Append(
            "refreshToken",
            result.Value.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            }
        );

        return Ok(new { accessToken = result.Value.AccessToken });
    }
}