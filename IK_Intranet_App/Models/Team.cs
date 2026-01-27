using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IK_Intranet_App.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Takım Adı")]
        public string Name { get; set; }

        // Davet Kodu: Rastgele oluşturulacak (Örn: A9X2-P3M1)
        public string InviteCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- İLİŞKİLER (Relations) ---

        // Bir takımın birden çok üyesi olabilir
        public ICollection<ApplicationUser> Members { get; set; }

        // Bir takımın birden çok görevi olabilir
        public ICollection<Gorev> Gorevler { get; set; }
    }
}