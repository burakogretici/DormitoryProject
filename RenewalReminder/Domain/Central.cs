using RenewalReminder.Domain.Enums;
using RenewalReminder.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace RenewalReminder.Domain
{
    public class Central : Permission
    {
        //[Display(Name = "Talebe Adı")]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        //public int StudentId { get; set; }

        [Display(Name = "Kimden İzin Aldı")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        public Staff Staff { get; set; }


        [Display(Name = "Nereye Gidiyor")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        public ToWhere ToWhere { get; set; }

        //[Display(Name = "Çıkış Saati")]
        //public DateTime? CheckOutTime { get; set; }

        //[Display(Name = "Giriş Saati")]
        ////[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        //public DateTime? CheckInTime { get; set; }

        [Display(Name = "Geçen Süre")]
        public int? ElapsedTime { get; set; }


        [Display(Name = "Mazeretli mi?")]
        public bool? IsExcused { get; set; }




        //public virtual Student? Student { get; set; }

    }
}
