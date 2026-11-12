
using ASP.NET_PlantsOrder.Data;
using ASP.NET_PlantsOrder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Mycon")));

// MVC
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Create or repair the development admin account
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<ApplicationDbContext>();

    var hasher = new PasswordHasher<Customer>();

    var admin = context.Customers
        .FirstOrDefault(c => c.Email == "admin@example.com");

    if (admin == null)
    {
        admin = new Customer
        {
            CustomerName = "Admin",
            Email = "admin@example.com",
            IsAdmin = true
        };

        admin.PasswordHash =
            hasher.HashPassword(admin, "Admin@123");

        context.Customers.Add(admin);
        context.SaveChanges();
    }
    else if (
        string.IsNullOrWhiteSpace(admin.PasswordHash) ||
        admin.PasswordHash == "REPLACE_WITH_A_VALID_HASH")
    {
        admin.IsAdmin = true;

        admin.PasswordHash =
            hasher.HashPassword(admin, "Admin@123");

        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Homee/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Homee}/{action=Index}/{id?}");

app.Run();
