using System.ComponentModel.DataAnnotations;
using dotnet_store.Attributes;

namespace dotnet_store.Models;


public class ProductEditModel
{
    public int Id { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = null!;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(10.0, 300000.0, ErrorMessage = "The {0} must be between {1} and {2}.")]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    public double? Price { get; set; }
    [Display(Name = "Active")]
    public bool IsActive { get; set; }
    [Display(Name = "Image")]
    public string? Image { get; set; }
    [Display(Name = "Image")]
    [ImageDimensions(1920, 1080)]
    public IFormFile? ImageFile { get; set; }
    [Display(Name = "Home")]
    public bool IsHome { get; set; }

    [Required(ErrorMessage = "Stock is required.")]
    [Range(0, 10000, ErrorMessage = "The {0} must be between {1} and {2}.")]
    [Display(Name = "Stock")]
    public int? Stock { get; set; }

    [Required(ErrorMessage = "Category selection is required.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
}