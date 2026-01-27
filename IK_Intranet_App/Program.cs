using IK_Intranet_App.Data;
using IK_Intranet_App.Models;
using IK_Intranet_App.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// PostgreSQL'in tarih saat konusundaki katı kuralını esnetir
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
// Veritaban� servisini projeye ekliyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Email onayı zorunluluğunu kaldıralım (Test için)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3; // Şifre politikalarını gevşettik (Geliştirme kolaylığı için)
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Login/Register sayfalarının çalışması için şart

// Standart ClaimsFactory yerine bizimkini kullan:
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();

// Kullanıcının kimliğine erişmek için gerekli (TenantService içinde kullandık)
builder.Services.AddHttpContextAccessor();

// Bizim yazdığımız Kiracı Servisi
builder.Services.AddScoped<ITenantService, TenantService>();

// Razor Pages teknolojisini projeye tanıtıyoruz.
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor sayfalarının adreslerini haritalıyoruz.
app.MapRazorPages();

// Rol ve Admin Oluşturma (Seeding)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Rolleri Tanımla
        string[] roleNames = { "Admin", "Member" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Rol yoksa oluştur (Veritabanına Admin ve Member rollerini ekler)
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Admin Kullanıcısı Oluştur (Eğer yoksa)
        var adminEmail = "admin@sirket.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                AdSoyad = "Sistem Yöneticisi", // Admin'in adı
                EmailConfirmed = true
            };

            // Şifresi: "Sau123!" (Güçlü şifre zorunluluğu var)
            var createAdmin = await userManager.CreateAsync(newAdmin, "Sau123!");

            if (createAdmin.Succeeded)
            {
                // Kullanıcı oluştuysa ona "Admin" rolünü ver
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Roller oluşturulurken bir hata meydana geldi.");
    }
}

// Rol Seeding İşlemi
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Az önce yazdığımız DbSeeder sınıfını çalıştırıyoruz
        await DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Roller oluşturulurken bir hata meydana geldi.");
    }
}

app.Run();
