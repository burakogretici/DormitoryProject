using KvsProject.Domain.Enums;
using KvsProject.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace KvsProject.Domain
{
    public class Student : Entity
    {
        [Display(Name = "Ad")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string Name { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string Surname { get; set; }

        [Display(Name = "Talebe Adı")]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string? FullName { get; set; }

        [Display(Name = "Yurt No")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        public int Number { get; set; }

        [Display(Name = "Telefon No")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(11, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string Phone { get; set; }

        //[Display(Name = "Talebe Görevi")]
        //public StudentType? StudentType { get; set; }

        public List<Central>? Centrals { get; set; }

    }
}

