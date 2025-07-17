using Microsoft.AspNetCore.Identity;

namespace dotnet_store.Models;


public class AppUser : IdentityUser<int>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}