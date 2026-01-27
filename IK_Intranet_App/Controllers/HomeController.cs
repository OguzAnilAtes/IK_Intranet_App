using IK_Intranet_App.Data;
using IK_Intranet_App.Models;
using IK_Intranet_App.Services; // <--- EKLENDÝ (TenantService için)
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace IK_Intranet_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantService _tenantService; // <--- EKLENDÝ

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<ApplicationUser> userManager, ITenantService tenantService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _tenantService = tenantService; // <--- EKLENDÝ
        }

        public IActionResult Index()
        {
            // 1. TAKIM BÝLGÝLERÝNÝ ÇEKME (Dashboard Baþlýðý ve Davet Kodu Ýçin)
            // -------------------------------------------------------------------
            var currentTeamId = _tenantService.GetTeamId();

            // Eðer takýmý YOKSA, View'a bunu bildir ve istatistik hesaplamadan çýk.
            if (currentTeamId == null)
            {
                ViewBag.UserHasTeam = false; // View bu bayraða bakýp ekraný deðiþtirecek
                return View();
            }
            else
            {
                ViewBag.UserHasTeam = true;

                // Takým ismini çek
                var team = _context.Teams.Find(currentTeamId.Value);
                ViewBag.TeamName = team?.Name;
                ViewBag.InviteCode = team?.InviteCode;
            }

            // 2. ÝSTATÝSTÝKLER (Global Query Filter Sayesinde Sadece Takým Verisi Gelir)
            // -------------------------------------------------------------------
            ViewBag.ToplamGorev = _context.Gorevler.Count();
            ViewBag.Tamamlanan = _context.Gorevler.Count(x => x.Durum == Enums.Durumlar.Tamamlandi);
            ViewBag.DevamEden = _context.Gorevler.Count(x => x.Durum != Enums.Durumlar.Tamamlandi);

            // 3. KÝÞÝSEL ÝSTATÝSTÝKLER (Bana Atananlar)
            // -------------------------------------------------------------------
            ViewBag.BenimGorevlerim = 0;
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);

                if (!string.IsNullOrEmpty(userId))
                {
                    ViewBag.BenimGorevlerim = _context.Gorevler
                        .Count(x => x.AppUserId == userId && x.Durum != Enums.Durumlar.Tamamlandi);
                }
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}