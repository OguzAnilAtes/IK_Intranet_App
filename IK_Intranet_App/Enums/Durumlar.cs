using System.ComponentModel.DataAnnotations;

namespace IK_Intranet_App.Enums
{
    public enum Durumlar
    {
        [Display(Name = "Yapılacak")]
        Yapilacak = 0,

        [Display(Name = "Sürüyor")]
        Suruyor = 1,

        [Display(Name = "Tamamlandı")]
        Tamamlandi = 2
    }
}