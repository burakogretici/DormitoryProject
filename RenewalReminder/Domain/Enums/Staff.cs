using System.ComponentModel.DataAnnotations;

namespace KvsProject.Domain.Enums
{
    public enum Staff
    {
        [Display(Name = "Seçiniz")]
        NONE = 0,
        [Display(Name = "Hasan Ö.")]
        Staff1,
        [Display(Name = "Süleyman Y.")]
        Staff2,
        [Display(Name = "Hamza T.")]
        Staff3,
        [Display(Name = "Enes B.")]
        Staff4,
        [Display(Name = "Sefa B.")]
        Staff5,
        [Display(Name = "Ahmet Kemal A.")]
        Staff6,
    }
}
