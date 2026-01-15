using IK_Intranet_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IK_Intranet_App.Data
{
    // IdentityDbContext sayesinde Users (Kullanıcılar), Roles (Roller) tablolarını otomatik tanıyacak.
    public class AppDbContext : IdentityDbContext<ApplicationUser> //Standart IdentityUser'a extra bilgiler (AdSoyad) eklemek için ApplicationUser ekledik
    {
        // Bu constructor (yapıcı metot), ayarları Program.cs'ten alır
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Veritabanındaki "Gorevler" tablosu bu koda karşılık gelir
        public DbSet<Gorev> Gorevler { get; set; }
    }
}