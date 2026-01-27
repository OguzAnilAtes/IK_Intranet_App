using IK_Intranet_App.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IK_Intranet_App.Models
{
    public class Gorev
    {
        [Key]
        public int Id { get; set; } // Her görevin benzersiz numarası

        [Required]
        public string Baslik { get; set; } = string.Empty; // Görevin adı

        public string? Aciklama { get; set; } // Detaylar (Boş olabilir)

        public string? AppUserId { get; set; } //İlişkinin Kimliği (Veritabanında tutulacak ID)


        [ForeignKey("AppUserId")]
        public ApplicationUser? AppUser { get; set; } //İlişkinin Kendisi (Kod yazarken kullanacağımız Nesne - Navigation Property)

        public string? OlusturanUserId { get; set; } // Bu alan "Görevin Sahibi"ni tutacak.

        public Durumlar Durum { get; set; } = Durumlar.Yapilacak; // Varsayılan: Yapılacak

        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        public int TeamId { get; set; } // Görev mutlaka bir takıma ait olmalı!

        public Team Team { get; set; } // İlişki
    }
}