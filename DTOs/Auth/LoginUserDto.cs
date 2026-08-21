using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs.Auth;

public class LoginUserDto
{
    [Required]
    public string Username { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
}