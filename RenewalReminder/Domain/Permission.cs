using KvsProject.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace KvsProject.Domain
{
    public class Permission : Entity
    {
        [Display(Name = "Öğrenci Adı")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        public int StudentId { get; set; }

        [Display(Name = "Çıkış Saati")]
        public DateTime? CheckOutTime { get; set; }

        [Display(Name = "Giriş Saati")]
        public DateTime? CheckInTime { get; set; }

        public virtual Student? Student { get; set; }

    }
}
