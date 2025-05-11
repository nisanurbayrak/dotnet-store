
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Slider> Sliders { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Slider>().HasData(
            new List<Slider>{
                new Slider {Id=1, Title="Title 1", Description="Description 1", Index=3, Active=true, Image="slider-1.jpeg"},
                new Slider {Id=2, Title="Title 2", Description="Description 2", Index=2, Active=true,Image="slider-2.jpeg"},
                new Slider {Id=3, Title="Title 3", Description="Description 3", Index=1, Active=false,Image="slider-3.jpeg"},
            }
        );

        modelBuilder.Entity<Category>().HasData(
            new List<Category>{
                new Category {Id=1, CategoryName="Saat", Url="saat"},
                new Category {Id=2, CategoryName="Telefon", Url="telefon"},
                new Category {Id=3, CategoryName="Beyaz Eşya", Url="beyaz-esya"},
                new Category {Id=4, CategoryName="Bilgisayar", Url="bilgisayar"},
                new Category {Id=5, CategoryName="Diğer", Url="diger"},
            }
        );

        modelBuilder.Entity<Product>().HasData(
            new List<Product>() {

                new Product(){Id = 1, ProductName="Product 1", Price=25350, IsActive=true, Image="1.jpeg", Description="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a fringilla magna. Duis ullamcorper volutpat nisl ac consequat. Suspendisse in dapibus tortor, at congue tortor.", IsHome=true, CategoryId=1},

                new Product(){Id = 2, ProductName="Product 2", Price=35350, IsActive=true, Image="2.jpeg", Description="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a fringilla magna. Duis ullamcorper volutpat nisl ac consequat. Suspendisse in dapibus tortor, at congue tortor.", IsHome=false, CategoryId=1},

                new Product(){Id = 3, ProductName="Product 3", Price=45350, IsActive=false, Image="3.jpeg", Description="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a fringilla magna. Duis ullamcorper volutpat nisl ac consequat. Suspendisse in dapibus tortor, at congue tortor.", IsHome=true, CategoryId=2},
            }
        );
    }
}

// Connection String
// Migrations

// DataContext _context = new DataContext();