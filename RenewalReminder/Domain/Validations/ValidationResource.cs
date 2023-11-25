using System;
namespace KvsProject.Domain.Validations
{
    internal class ValidationResource
    {
        public static string CustomValidation => "{0} geçersiz.";

        public static string Range => "{0} {1} ile {2} arasında olmalıdır.";

        public static string Regex => "{0} {1} kuralına uymalıdır.";

        public static string Required => "{0} boş geçilemez.";

        public static string StringLength => "{0} için karakter uzunluğu {1} ile {2} arasında olamalıdır.";

        public static string Validation => "{0} geçersiz.";

        public static string MaxLength => "{0} en fazla {1} karakter uzunluğunda olabilir.";

        public static string MinLength => "{0} en az {1} karakter uzunluğunda olmalıdır.";

        public static string CreditCard => "{0} geçereli bir kredi kartı numarası değildir.";

        public static string EmailAddress => "{0} geçerli bir e-posta adresi değildir.";

        public static string Url => "{0} geçerli bir link değildir.";

        public static string Compare => "{0} ile {1} aynı olmalıdır";
    }
}
