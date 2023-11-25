//using RenewalReminder.Domain.Validations;
//using System.ComponentModel.DataAnnotations;

//namespace RenewalReminder.Domain
//{
//    public class StudentCentral : Entity
//    {
//        [Display(Name = "Talebe")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        public int StudentId { get; set; }

//        [Display(Name = "İzin")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        public int CentralId { get; set; }

//        public Student? Student { get; set; }
//        public Central? Central { get; set; }
//    }
//}
