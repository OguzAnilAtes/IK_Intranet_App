using IK_Intranet_App.Models;
using IK_Intranet_App.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IK_Intranet_App.Data
{
    // IdentityDbContext sayesinde Users (Kullanıcılar), Roles (Roller) tablolarını otomatik tanıyacak.
    public class AppDbContext : IdentityDbContext<ApplicationUser> //Standart IdentityUser'a extra bilgiler (AdSoyad) eklemek için ApplicationUser ekledik
    {
        private readonly ITenantService _tenantService; // Kiracı servisini burada tutacağız

        // Bu constructor (yapıcı metot), ayarları Program.cs'ten alır
        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantService tenantService)
            : base(options)
        {
            _tenantService = tenantService;
        }

        // Veritabanındaki "Gorevler" tablosu bu koda karşılık gelir
        public DbSet<Gorev> Gorevler { get; set; }
        public DbSet<Team> Teams { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Gorev tablosuna her sorgu atıldığında bu kuralı uygula:
            builder.Entity<Gorev>().HasQueryFilter(g =>  // GLOBAL QUERY FILTER
                g.TeamId == _tenantService.GetTeamId());
        }
    }
}