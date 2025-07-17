using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class UserCreateModel
{
    public string Username { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}