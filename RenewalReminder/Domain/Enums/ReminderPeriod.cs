using System.ComponentModel.DataAnnotations;

namespace RenewalReminder.Domain.Enums
{
    public enum ReminderPeriod : int
    {
        [Display(Name = "Seçiniz")]
        NONE = 0,

        [Display(Name = "Günlük")]
        Daily = 1,

        [Display(Name = "Haftalık")]
        Weekly = 2,

        [Display(Name = "Aylık")]
        Monthly = 3,

        [Display(Name = "Yıllık")]
        Yearly = 4
    }
}
