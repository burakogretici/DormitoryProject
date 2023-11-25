using RenewalReminder.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace RenewalReminder.Models
{
    public class UserForLogin
    {
        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string Username { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
        public string Password { get; set; }
    }
}
