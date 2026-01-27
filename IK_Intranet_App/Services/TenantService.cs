using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IK_Intranet_App.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetTeamId()
        {
            // O anki kullanıcının kimlik kartına bak
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return null; // Kullanıcı giriş yapmamışsa takım da yoktur
            }

            // "TeamId" damgasını oku
            var teamIdClaim = user.FindFirst("TeamId");

            if (teamIdClaim != null && int.TryParse(teamIdClaim.Value, out int teamId))
            {
                return teamId; // Buldum! Takım ID'si bu.
            }

            return null;
        }
    }
}