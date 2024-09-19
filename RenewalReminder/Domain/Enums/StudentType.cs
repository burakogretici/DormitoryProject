using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvsProject.Domain.Enums
{
    public enum StudentType
    {
        [Display(Name = "Seçiniz")]
        NONE = 0,
        [Display(Name = "Nöbetçi")]
        Guard,
        [Display(Name = "Çaycı")]
        Teaci
    }
    public enum Status
    {
        
        [Display(Name = "Evet")]
        Yes,
        [Display(Name = "Hayır")]
        No
    }

}
