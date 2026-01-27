using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IK_Intranet_App.Models
{
    // Standart IdentityUser'dan miras alıyoruz, yani onun her şeyi bunda da var.
    // Artı olarak AdSoyad ekliyoruz.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string AdSoyad { get; set; } = string.Empty;

        public int? TeamId { get; set; } // Hangi takıma ait? (Nullable yapıyoruz ki ilk başta hatasız oluşsun)

        public Team Team { get; set; } // İlişki (Navigation Property)
    }
}