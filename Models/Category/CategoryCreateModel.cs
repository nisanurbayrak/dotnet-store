using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class CategoryCreateModel
{
    [Required]
    [StringLength(50)]
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = null!;
    [Required]
    [StringLength(20)]
    [Display(Name = "Url")]
    public string Url { get; set; } = null!;
    [Required]
    [Display(Name = "Active")]
    public bool IsActive { get; set; }

}