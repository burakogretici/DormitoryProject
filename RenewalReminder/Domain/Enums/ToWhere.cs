using System.ComponentModel.DataAnnotations;

namespace RenewalReminder.Domain.Enums
{
    public enum ToWhere 
    {
        [Display(Name = "Seçiniz")]
        NONE = 0,
        [Display(Name = "Okul")]
        School,
        [Display(Name = "Hastane")]
        Hospital,
        [Display(Name = "Diğer")]
        Other
    }

}
