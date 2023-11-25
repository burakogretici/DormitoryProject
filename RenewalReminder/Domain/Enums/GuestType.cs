using System.ComponentModel.DataAnnotations;

namespace KvsProject.Domain.Enums
{
    public enum GuestType
    {
        [Display(Name = "Seçiniz")]
        NONE = 0,
        [Display(Name = "Hocaefendi")]
        Hoca,
        [Display(Name = "Talebe")]
        Student,
        [Display(Name = "Diğer")]
        Other
    }

}
