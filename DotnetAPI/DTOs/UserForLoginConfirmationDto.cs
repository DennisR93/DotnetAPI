namespace DotnetAPI.DTOs;

public class UserForLoginConfirmationDto
{
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
}