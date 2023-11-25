//using RenewalReminder.Domain.Validations;
//using System.ComponentModel.DataAnnotations;

//namespace RenewalReminder.Domain
//{
//    [Serializable]
//    public class UserRenewalReminder : Entity
//    {
//        [Display(Name = "Kullanıcı")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        public int UserId { get; set; }

//        [Display(Name = "Hatırlatıcı")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        public int RenewalReminderId { get; set; }

//        public User? User { get; set; }
//        public RenewalReminder? RenewalReminder { get; set; }
//    }
//}
