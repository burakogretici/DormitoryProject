using KvsProject.Domain;
using System.ComponentModel.DataAnnotations;

namespace KvsProject.Models
{
    public class StudentPermits 
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }

        public List<PermitDetail> PermitDetails { get; set; }

        [Display(Name = "Çıkış Saati")]
        public DateTime? CheckOutTime { get; set; }

        [Display(Name = "Giriş Saati")]
        public DateTime? CheckInTime { get; set; }

        [Display(Name = "Toplam Süre")]
        public int TotalLeave { get; set; }

        [Display(Name = "Mazeretli mi?")]
        public bool? IsExcused { get; set; }

        public int ElapsedTime { get; set; }
        public int StudentNumber { get; set; }


        public Student? Student { get; set; }
    }

    public class PermitDetail
    {
        //public DateTime Date { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? CheckInTime { get; set; }
    }


}
