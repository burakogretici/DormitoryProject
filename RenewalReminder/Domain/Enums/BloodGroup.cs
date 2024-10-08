﻿using System.ComponentModel.DataAnnotations;

namespace  KvsProject.Domain.Enums
{
    public enum BloodGroup
    {
        [Display(Name = "Seçiniz")]
        NONE = 0,

        [Display(Name = "A+")]
        APositive,

        [Display(Name = "A-")]
        ANegative,

        [Display(Name = "B+")]
        BPositive,

        [Display(Name = "B-")]
        BNegative,

        [Display(Name = "AB+")]
        ABPositive,

        [Display(Name = "AB-")]
        ABNegative,

        [Display(Name = "O+")]
        OPositive,

        [Display(Name = "O-")]
        ONegative
    }

}
