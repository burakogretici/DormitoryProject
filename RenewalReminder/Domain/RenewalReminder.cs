//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using RenewalReminder.Domain.Enums;
//using RenewalReminder.Domain.Validations;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace RenewalReminder.Domain
//{
//    [Serializable]
//    public class RenewalReminder  : Entity
//    {
//        [Display(Name = "Başlık")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
//        public string Title { get; set; }

//        [Display(Name = "Açıklama")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        [MaxLength(400, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationResource))]
//        public string Description { get; set; }

//        [Display(Name = "Hatırlatma Periyodu")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        public ReminderPeriod ReminderPeriod { get; set; }

//        [Display(Name = "Başlangıç Tarihi")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        public DateTime StartDate { get; set; }

//        [Display(Name = "Bitiş Tarihi")]
//        public DateTime? EndDate { get; set; }

//        public List<UserRenewalReminder>? UserRenewalReminders { get; set; }
//    }
//}
