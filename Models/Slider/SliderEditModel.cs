using System.ComponentModel.DataAnnotations;
using dotnet_store.Attributes;
namespace dotnet_store.Models;


public class SliderEditModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    [Display(Name = "Image")]
    public string? Image { get; set; }
    [Display(Name = "Image")]
    [ImageDimensions(1920, 1080)]
    public IFormFile? ImageFile { get; set; }
    public bool Active { get; set; }
    public int Index { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
