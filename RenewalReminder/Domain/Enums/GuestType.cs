﻿using System.ComponentModel.DataAnnotations;

namespace KvsProject.Domain.Enums
{
    public enum GuestType
    {
        [Display(Name = "Seçiniz")]
        NONE = 0,
        [Display(Name = "Misafir")]
        Hoca,
        [Display(Name = "Öğrenci")]
        Student,
        [Display(Name = "Diğer")]
        Other
    }

}
