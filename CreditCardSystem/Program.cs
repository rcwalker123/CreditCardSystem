using CreditCardSystem.Interfaces;
using CreditCardSystem.Models;
using CreditCardSystem.ProcessManagers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var connectionString = builder.Configuration.GetConnectionString("AppDb");

builder.Services.AddDbContext<CreditCardSystemContext>(options =>
    options.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=CreditCardSystem;Integrated Security=True;TrustServerCertificate=True"),
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
