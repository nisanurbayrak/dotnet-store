using System.ComponentModel.DataAnnotations;
using dotnet_store.Attributes;
namespace dotnet_store.Models;


public class SliderCreateModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    [ImageDimensions(1920, 1080)]
    public IFormFile? Image { get; set; }
    public bool Active { get; set; }
    public int Index { get; set; }
}
