using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using RestaurantWebApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys")));

builder.Services.AddDbContext<AlohaTableDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AlohaTableDb") ?? "Data Source=AlohaTable.db"));

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".RestaurantWebApp.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AlohaTableDbContext>();
    db.Database.EnsureCreated();

    if (!db.Logins.Any())
    {
        db.Logins.Add(new Login { UserName = "admin", UserPassword = "admin" });
    }

    if (!db.Categories.Any())
    {
        db.Categories.AddRange(
            new Category { CategoryCode = "LUNCH", CategoryName = "Lunch" },
            new Category { CategoryCode = "DINNER", CategoryName = "Dinner" },
            new Category { CategoryCode = "KIDS", CategoryName = "Kids Meal" },
            new Category { CategoryCode = "PUPUS", CategoryName = "Pupus" },
            new Category { CategoryCode = "DESSERT", CategoryName = "Dessert" },
            new Category { CategoryCode = "BEVERAGE", CategoryName = "Beverage" }
        );
    }

    db.SaveChanges();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
