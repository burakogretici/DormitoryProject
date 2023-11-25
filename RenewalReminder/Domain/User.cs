using KvsProject.Domain.Enums;
using KvsProject.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace KvsProject.Domain
{
    [Serializable]
    public class User : Entity
    {
        [Display(Name = "Ad")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string Name { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string Surname { get; set; }

        [Display(Name = "Kullanıcı Tipi")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        public UserType UserType { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessageResourceName = "Required")]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string Username { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(40, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string Password { get; set; }

    }
}
