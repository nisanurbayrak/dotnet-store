using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class AccountLoginModel
{
    // public string Username { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    public bool IsConfirmed { get; set; }
    public bool RememberMe { get; set; } = true;
}