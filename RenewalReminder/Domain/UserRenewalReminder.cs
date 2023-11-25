//using KvsProject.Domain.Validations;
//using System.ComponentModel.DataAnnotations;

//namespace KvsProject.Domain
//{
//    [Serializable]
//    public class UserKvsProject : Entity
//    {
//        [Display(Name = "Kullanıcı")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        public int UserId { get; set; }

//        [Display(Name = "Hatırlatıcı")]
//        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResource))]
//        public int KvsProjectId { get; set; }

//        public User? User { get; set; }
//        public KvsProject? KvsProject { get; set; }
//    }
//}
