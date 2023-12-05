using System.ComponentModel.DataAnnotations;

namespace KvsProject.Domain.Enums
{
    public enum UserType : byte
    {
        [Display(Name = "Seçiniz")]
        NONE = 0,

        [Display(Name = "Admin")]
        ADMIN = 1,

        [Display(Name = "Santralci")]
        CENTER = 2,

        [Display(Name = "Başkan")]
        MİNİSTER = 3,
        
    }
}
