using KvsProject.Domain.Enums;
using KvsProject.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace KvsProject.Domain
{
    public class Guest : Entity
    {
        [Display(Name = "Misafir Adı")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string FullName { get; set; }

        [Display(Name = "Niçin Geldi")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string WhyCome { get; set; }

        [Display(Name = "Kiminle Görüşecek")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string WhoCome { get; set; }


        [Display(Name = "Nereden Geldi")]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string? FromWhere { get; set; }

        [Display(Name = "Durumu")]
        public GuestType? GuestType { get; set; }
    }
}
