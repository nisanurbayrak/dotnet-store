namespace dotnet_store.Models;


public class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; } = null!;
    public double? Price { get; set; }
    public bool IsActive { get; set; }
    public string? Image { get; set; }
    public bool IsHome { get; set; }
    public string? Description { get; set; }
    public int Stock { get; set; }

    public Category? Category { get; set; } = null!;
    public List<ProductCategory> ProductCategories { get; set; } = new();

}