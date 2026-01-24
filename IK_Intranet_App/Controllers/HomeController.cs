using IK_Intranet_App.Data;
using IK_Intranet_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IK_Intranet_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context; // 1. Veritabaný baðlantýsý
        private readonly UserManager<ApplicationUser> _userManager; //Kullanýcý yöneticisini tanýmlýyoruz

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            // Genel Ýstatistikleri Hesapla
            ViewBag.ToplamGorev = _context.Gorevler.Count();
            ViewBag.Tamamlanan = _context.Gorevler.Count(x => x.Durum == Enums.Durumlar.Tamamlandi);
            ViewBag.DevamEden = _context.Gorevler.Count(x => x.Durum != Enums.Durumlar.Tamamlandi);

            // Kiþiye özel atanan istatistikler
            ViewBag.BenimGorevlerim = 0;
            if (User.Identity.IsAuthenticated)
            {
                // Giriþ yapan kullanýcýnýn ID'sini (GUID) alýyoruz
                var userId = _userManager.GetUserId(User);

                // Veritabanýnda "AppUserId" sütunu, benim ID'm olanlarý say
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
