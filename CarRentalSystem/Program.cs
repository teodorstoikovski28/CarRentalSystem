using CarRentalSystem.Data;
using CarRentalSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category { Name = "SUV" },
            new Category { Name = "Sedan" },
            new Category { Name = "Electric" },
            new Category { Name = "Sports" },
            new Category { Name = "Luxury" }
        );

        context.SaveChanges();
    }

    if (!context.Cars.Any())
    {
        context.Cars.AddRange(
            new Car
            {
                Brand = "BMW",
                Model = "M5",
                Year = 2024,
                PricePerDay = 270,
                CategoryId = context.Categories.First(c => c.Name == "Sports").Id
            },
            new Car
            {
                Brand = "Mercedes",
                Model = "C63 AMG",
                Year = 2024,
                PricePerDay = 250,
                CategoryId = context.Categories.First(c => c.Name == "Luxury").Id
            },
            new Car
            {
                Brand = "Audi",
                Model = "RS7",
                Year = 2024,
                PricePerDay = 260,
                CategoryId = context.Categories.First(c => c.Name == "Sports").Id
            },
            new Car
            {
                Brand = "Tesla",
                Model = "Model S",
                Year = 2024,
                PricePerDay = 220,
                CategoryId = context.Categories.First(c => c.Name == "Electric").Id
            },
            new Car
            {
                Brand = "Lamborghini",
                Model = "Huracan",
                Year = 2024,
                PricePerDay = 500,
                CategoryId = context.Categories.First(c => c.Name == "Luxury").Id
            }
        );

        context.SaveChanges();
    }

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }

    var adminEmail = "admin@gmail.com";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "System"
        };

        await userManager.CreateAsync(adminUser, "Admin123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cars}/{action=Index}/{id?}");

app.Run();