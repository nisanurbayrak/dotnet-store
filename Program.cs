using Microsoft.AspNetCore.Identity;
using dotnet_store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using dotnet_store.Services;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<DataContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});

builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();
builder.Services.AddSession();

builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));

builder.Services.AddScoped<EmailSender>(provider =>
{
    var options = provider.GetRequiredService<IOptions<SendGridSettings>>().Value;
    return new EmailSender(options.ApiKey, options.FromEmail, options.FromName);
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

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
