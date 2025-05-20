namespace dotnet_store.Models;

//entity 
//veri tabanı tablosu için
public class Category
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<ProductCategory> ProductCategories { get; set; } = new();
}