using IK_Intranet_App.Data;
using IK_Intranet_App.Models;
using IK_Intranet_App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IK_Intranet_App.Controllers
{
    [Authorize]
    public class TeamController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITenantService _tenantService;
        private readonly AppDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager; // Çerezi yenilemek için

        public TeamController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITenantService tenantService, AppDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tenantService = tenantService;
            _context = context;
            _signInManager = signInManager;
        }

        // GET: /Team/Index
        public async Task<IActionResult> Index()
        {
            var currentTeamId = _tenantService.GetTeamId();
            if (currentTeamId == null) return View("Error", new ErrorViewModel { RequestId = "NoTeam" });

            // 1. Takımdaki kullanıcıları çek
            var teamUsers = await _userManager.Users
                .Where(u => u.TeamId == currentTeamId)
                .ToListAsync();

            // 2. ViewModel Listesi Hazırla (Logic View'da değil burada olacak)
            var model = new List<TeamMemberViewModel>();

            foreach (var user in teamUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.Contains("Admin") ? "Yönetici" : "Üye";

                model.Add(new TeamMemberViewModel
                {
                    Id = user.Id,
                    AdSoyad = user.AdSoyad,
                    Email = user.Email,
                    Role = primaryRole,
                    IsAdmin = roles.Contains("Admin")
                });
            }

            return View(model);
        }

        // POST: /Team/Kick/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Kick(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var targetUser = await _userManager.FindByIdAsync(userId);

            if (targetUser == null) return NotFound();

            // GÜVENLİK: Başkasının adamını atamasın
            if (targetUser.TeamId != currentUser.TeamId) return Forbid();

            // GÜVENLİK: Kendini atamasın
            if (targetUser.Id == currentUser.Id)
            {
                TempData["Error"] = "Kendinizi takımdan atamazsınız!";
                return RedirectToAction(nameof(Index));
            }

            // --- PROFESYONEL TEMİZLİK ---
            // 1. Kişinin rollerini sil
            var roles = await _userManager.GetRolesAsync(targetUser);
            if (roles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(targetUser, roles);
            }

            // 2. Takımdan çıkar
            targetUser.TeamId = null;
            await _userManager.UpdateAsync(targetUser);

            TempData["Success"] = $"{targetUser.AdSoyad} takımdan çıkarıldı ve yetkileri alındı.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Team/ChangeRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            // newRole sadece "Admin" veya "Member" olabilir.
            if (newRole != "Admin" && newRole != "Member") return BadRequest();

            var currentUser = await _userManager.GetUserAsync(User);
            var targetUser = await _userManager.FindByIdAsync(userId);

            if (targetUser == null) return NotFound();
            if (targetUser.TeamId != currentUser.TeamId) return Forbid();
            if (targetUser.Id == currentUser.Id)
            {
                TempData["Error"] = "Kendi rolünüzü değiştiremezsiniz!";
                return RedirectToAction(nameof(Index));
            }

            // --- ROL GÜNCELLEME ---
            // Önce mevcut tüm rolleri sil, sonra yenisini ekle (Temiz İş)
            var currentRoles = await _userManager.GetRolesAsync(targetUser);
            await _userManager.RemoveFromRolesAsync(targetUser, currentRoles);
            await _userManager.AddToRoleAsync(targetUser, newRole);

            TempData["Success"] = $"{targetUser.AdSoyad} kullanıcısının rolü '{newRole}' olarak güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Team/Settings
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Settings()
        {
            var currentTeamId = _tenantService.GetTeamId();
            if (currentTeamId == null) return NotFound();

            var team = await _context.Teams.FindAsync(currentTeamId);
            if (team == null) return NotFound();

            return View(team);
        }

        // POST: /Team/UpdateName
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateName(string teamName)
        {
            var currentTeamId = _tenantService.GetTeamId();
            if (currentTeamId == null) return NotFound();

            if (string.IsNullOrWhiteSpace(teamName))
            {
                TempData["Error"] = "Takım adı boş olamaz!";
                return RedirectToAction(nameof(Settings));
            }

            var team = await _context.Teams.FindAsync(currentTeamId);
            if (team != null)
            {
                team.Name = teamName;
                _context.Update(team);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Takım ismi başarıyla güncellendi.";
            }

            return RedirectToAction(nameof(Settings));
        }

        // POST: /Team/CreateTeam (Giriş yapmış kullanıcı için)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeam(string teamName)
        {
            if (string.IsNullOrWhiteSpace(teamName)) return RedirectToAction("Index", "Home");

            // 1. Yeni Takım Oluştur
            var newTeam = new Team
            {
                Name = teamName,
                InviteCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                CreatedAt = DateTime.UtcNow
            };
            _context.Teams.Add(newTeam);
            await _context.SaveChangesAsync(); // ID oluşsun

            // 2. Kullanıcıyı bu takıma Yönetici olarak ata
            var user = await _userManager.GetUserAsync(User);

            // Önce eski bağları temizle (Eğer varsa)
            user.TeamId = newTeam.Id;
            await _userManager.UpdateAsync(user);

            // Rolleri temizle ve Admin yap
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, "Admin");

            await _signInManager.RefreshSignInAsync(user); // Çerezi yenilemek için

            return RedirectToAction("Index", "Home");
        }

        // POST: /Team/JoinTeam (Giriş yapmış kullanıcı için)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinTeam(string inviteCode)
        {
            if (string.IsNullOrWhiteSpace(inviteCode)) return RedirectToAction("Index", "Home");

            // 1. Takımı Bul
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.InviteCode == inviteCode);
            if (team == null)
            {
                TempData["Error"] = "Geçersiz davet kodu!";
                return RedirectToAction("Index", "Home");
            }

            // 2. Kullanıcıyı bu takıma Üye olarak ata
            var user = await _userManager.GetUserAsync(User);

            user.TeamId = team.Id;
            await _userManager.UpdateAsync(user);

            // Rolleri temizle ve Member yap
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, "Member");

            await _signInManager.RefreshSignInAsync(user); // Çerezi yenilemek için

            return RedirectToAction("Index", "Home");
        }

        // POST: /Team/RegenerateInviteCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> RegenerateInviteCode() // Birini attıktan sonra tekrar katılamasın diye yeni kod üretebilmek için
        {
            var currentTeamId = _tenantService.GetTeamId();
            if (currentTeamId == null) return NotFound();

            var team = await _context.Teams.FindAsync(currentTeamId);
            if (team == null) return NotFound();

            // Yeni bir kod üret
            team.InviteCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            _context.Update(team);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Davet kodu yenilendi. Eski kod artık geçersiz.";
            return RedirectToAction(nameof(Settings));
        }
    }
}