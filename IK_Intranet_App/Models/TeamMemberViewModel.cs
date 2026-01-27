namespace IK_Intranet_App.Models
{
    public class TeamMemberViewModel
    {
        public string Id { get; set; }
        public string AdSoyad { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // "Admin" veya "Member"
        public bool IsAdmin { get; set; } // View'da kolay kontrol için
    }
}