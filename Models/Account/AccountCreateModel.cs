using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class AccountCreateModel
{
    public int Id { get; set; }
    [Required]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Please enter only letters and numbers.")]
    public string Username { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password confirmation does not match.")]
    public string ConfirmPassword { get; set; } = null!;
    public bool IsConfirmed { get; set; }
}