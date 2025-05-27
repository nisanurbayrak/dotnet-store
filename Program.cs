using dotnet_store.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// addcontroller api kullanacağın zaman kullanırsın.


builder.Services.AddDbContext<DataContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// app.MapStaticAssets();

// app.MapControllerRoute(
//     name: "products_by_category",
//     pattern: "products/{url?}",
//     defaults: new { controller = "Product", action = "List" })
//     .WithStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
