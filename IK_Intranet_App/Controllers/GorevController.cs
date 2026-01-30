using IK_Intranet_App.Data;
using IK_Intranet_App.Models;
using IK_Intranet_App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IK_Intranet_App.Controllers
{
    [Authorize]
    public class GorevController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantService _tenantService;

        public GorevController(AppDbContext context, UserManager<ApplicationUser> userManager, ITenantService tenantService)
        {
            _context = context;
            _userManager = userManager;
            _tenantService = tenantService;
        }

        // GET: Gorev
        public async Task<IActionResult> Index()
        {
            var gorevler = await _context.Gorevler
                             .Include(x => x.AppUser)
                             .OrderByDescending(x => x.OlusturmaTarihi) // En yeniden en eskiye sırala
                             .ToListAsync();
            return View(gorevler);
        }

        // GET: Gorev/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gorev = await _context.Gorevler
                .Include(g => g.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gorev == null) return NotFound();

            return View(gorev);
        }

        // GET: Gorev/Create
        public IActionResult Create()
        {
            // GÜNCELLEME 1: Sadece kendi takımındaki kullanıcıları listele
            var currentTeamId = _tenantService.GetTeamId();
            var teamUsers = _userManager.Users.Where(u => u.TeamId == currentTeamId).ToList();

            ViewBag.Users = new SelectList(teamUsers, "Id", "AdSoyad");
            return View();
        }

        // POST: Gorev/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Baslik,Aciklama,AppUserId,Durum")] Gorev gorev)
        {
            ModelState.Remove("AppUser");
            ModelState.Remove("OlusturanUserId");

            // 1. ADIM: Takım ID Ataması
            var currentTeamId = _tenantService.GetTeamId();

            if (currentTeamId.HasValue)
            {
                gorev.TeamId = currentTeamId.Value;
            }
            else
            {
                ModelState.AddModelError("", "Bir takıma ait olmadığınız için görev oluşturamazsınız.");
            }

            // 2. ADIM: Validation Temizliği
            ModelState.Remove("Team");
            ModelState.Remove("TeamId");

            if (ModelState.IsValid)
            {
                var currentUserId = _userManager.GetUserId(User);
                gorev.OlusturanUserId = currentUserId;
                gorev.OlusturmaTarihi = DateTime.UtcNow;

                _context.Add(gorev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Hata Durumu: Listeyi tekrar (Filtreli olarak) doldur
            if (currentTeamId.HasValue)
            {
                var teamUsers = _userManager.Users.Where(u => u.TeamId == currentTeamId).ToList();
                ViewBag.Users = new SelectList(teamUsers, "Id", "AdSoyad");
            }

            return View(gorev);
        }

        // GET: Gorev/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gorev = await _context.Gorevler.FindAsync(id);
            if (gorev == null) return NotFound();

            // YETKİ KONTROLÜ
            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole("Admin");
            bool isOlusturan = gorev.OlusturanUserId == currentUserId;
            bool isAtanan = gorev.AppUserId == currentUserId;

            if (!isAdmin && !isOlusturan && !isAtanan)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
            }

            // GÜNCELLEME 2: Düzenlerken de sadece takım arkadaşlarını gör
            var currentTeamId = _tenantService.GetTeamId();
            var teamUsers = _userManager.Users.Where(u => u.TeamId == currentTeamId).ToList();

            ViewBag.Users = new SelectList(teamUsers, "Id", "AdSoyad", gorev.AppUserId);
            return View(gorev);
        }

        // POST: Gorev/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Baslik,Aciklama,AppUserId,Durum,OlusturmaTarihi")] Gorev gorev)
        {
            if (id != gorev.Id) return NotFound();

            // Orijinal veriyi takip etmeden (AsNoTracking) çekiyoruz
            var originalGorev = await _context.Gorevler.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (originalGorev == null) return NotFound();

            // YETKİ KONTROLÜ
            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole("Admin");
            bool isOlusturan = originalGorev.OlusturanUserId == currentUserId;
            bool isAtanan = originalGorev.AppUserId == currentUserId;

            if (!isAdmin && !isOlusturan && !isAtanan)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
            }

            // GÜNCELLEME 3: Validation Temizliği (TeamId için)
            ModelState.Remove("AppUser");
            ModelState.Remove("OlusturanUserId");
            ModelState.Remove("Team");      // <-- EKLENDİ
            ModelState.Remove("TeamId");    // <-- EKLENDİ

            if (ModelState.IsValid)
            {
                try
                {
                    // 3. ADIM: Kritik verileri koru 🛡️
                    gorev.OlusturanUserId = originalGorev.OlusturanUserId;
                    gorev.OlusturmaTarihi = originalGorev.OlusturmaTarihi;

                    // GÜNCELLEME 4: Takım bilgisini kaybetme!
                    gorev.TeamId = originalGorev.TeamId;

                    _context.Update(gorev);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GorevExists(gorev.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Hata Durumu: Listeyi tekrar (Filtreli olarak) doldur
            var currentTeamId = _tenantService.GetTeamId();
            var teamUsers = _userManager.Users.Where(u => u.TeamId == currentTeamId).ToList();
            ViewBag.Users = new SelectList(teamUsers, "Id", "AdSoyad", gorev.AppUserId);

            return View(gorev);
        }

        // GET: Gorev/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gorev = await _context.Gorevler
                .Include(g => g.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gorev == null) return NotFound();

            // YETKİ KONTROLÜ
            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole("Admin");
            bool isOlusturan = gorev.OlusturanUserId == currentUserId;

            if (!isAdmin && !isOlusturan)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
            }

            return View(gorev);
        }

        // POST: Gorev/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gorev = await _context.Gorevler.FindAsync(id);

            // YETKİ KONTROLÜ
            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole("Admin");
            bool isOlusturan = gorev.OlusturanUserId == currentUserId;

            if (!isAdmin && !isOlusturan)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
            }

            if (gorev != null)
            {
                _context.Gorevler.Remove(gorev);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GorevExists(int id)
        {
            return _context.Gorevler.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDurum(int id, int durumId)
        {
            var gorev = await _context.Gorevler.FindAsync(id);
            if (gorev == null) return Json(new { success = false, message = "Görev bulunamadı" });

            var currentUser = await _userManager.GetUserAsync(User);

            // Yetki Kontrolü
            bool yetkili = User.IsInRole("Admin") ||
                           gorev.OlusturanUserId == currentUser.Id ||
                           gorev.AppUserId == currentUser.Id;

            if (!yetkili)
            {
                // Forbid() yerine JSON dönüyoruz. Böylece sistem login sayfasına yönlendirme yapmaz.
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            gorev.Durum = (IK_Intranet_App.Enums.Durumlar)durumId;
            _context.Update(gorev);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Durum güncellendi" });
        }
    }
}