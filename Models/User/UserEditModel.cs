using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class UserEditModel
{

    public string Username { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [DataType(DataType.Password)]
    public string? Password { get; set; } = null!;
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password confirmation does not match.")]
    public string? ConfirmPassword { get; set; } = null!;

    public IList<string>? SelectedRoles { get; set; }
}