using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace KvsProject.Domain.Enums
{
    public enum Staff
    {
        [Display(Name = "Seçiniz")]
        NONE,
        [Display(Name = "Hasan Özer")]
        Staff1,
        [Display(Name = "Süleyman Yüksel")]
        Staff2,
        [Display(Name = "Hamza Türkmen")]
        Staff3,
        [Display(Name = "Enes Baykara")]
        Staff4,
        [Display(Name = "Sefa Bayram")]
        Staff5,
        [Display(Name = "Ahmet Kemal Aksoy")]
        Staff6,
    }

   
}
