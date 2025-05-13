namespace dotnet_store.Models;

//model
//veri taşımak için
public class CategoryGetModel
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public bool IsActive { get; set; }
    public int ProductCount { get; set; } //navigation property//ürün listesi yerine geldi
}