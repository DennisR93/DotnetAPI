namespace DotnetAPI.DTOs;

public class UserForRegistrationDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string PasswordConfirm { get; set; } = "";
}