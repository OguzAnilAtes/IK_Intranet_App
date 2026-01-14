using System.ComponentModel.DataAnnotations;

namespace IK_Intranet_App.Models
{
    public class Gorev
    {
        [Key]
        public int Id { get; set; } // Her görevin benzersiz numarası

        [Required]
        public string Baslik { get; set; } = string.Empty; // Görevin adı

        public string? Aciklama { get; set; } // Detaylar (Boş olabilir)

        public string? AtananKisi { get; set; } // Kime atandı?

        public string Durum { get; set; } = "Yapılacak"; // Yapılacak, Sürüyor, Bitti

        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    }
}