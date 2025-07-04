namespace dotnet_store.Models;


public class SliderGetModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string Image { get; set; } = null!;
    public bool Active { get; set; }
    public int Index { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
