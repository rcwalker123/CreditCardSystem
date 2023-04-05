using CreditCardSystem.Interfaces;
using CreditCardSystem.Models;
using CreditCardSystem.ProcessManagers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

builder.Services.AddDbContext<CreditCardSystemContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")),
    optionsLifetime: ServiceLifetime.Scoped);

builder.Services.AddScoped<ICardValidator, CardValidator>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
