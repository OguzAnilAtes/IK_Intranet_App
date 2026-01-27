using IK_Intranet_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IK_Intranet_App.Services
{
    // Bu sınıf, kullanıcı giriş yaparken çalışır ve kimlik kartını (Claims) oluşturur.
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            // Standart kimlik bilgilerini (User ID, Name, Role vs.) oluştur
            var identity = await base.GenerateClaimsAsync(user);

            // --- EKSTRA BİLGİ EKLEME ---
            // Eğer kullanıcının bir takımı varsa, bunu kimlik kartına "TeamId" adıyla yaz.
            if (user.TeamId.HasValue)
            {
                identity.AddClaim(new Claim("TeamId", user.TeamId.Value.ToString()));
            }

            return identity;
        }
    }
}