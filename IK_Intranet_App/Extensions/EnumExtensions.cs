using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace IK_Intranet_App.Extensions
{
    public static class EnumExtensions
    {
        // Bu metod, Enum'un üzerindeki [Display] yazısını okur.
        // Eğer Display yoksa normal adını (Yapilacak) döndürür.
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()?
                            .GetName() ?? enumValue.ToString();
        }
    }
}