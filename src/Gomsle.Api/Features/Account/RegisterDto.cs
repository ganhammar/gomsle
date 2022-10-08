using System.ComponentModel.DataAnnotations;

namespace Gomsle.Api.Features.Account;

public class RegisterDto
{
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? ReturnUrl { get; set; }
}