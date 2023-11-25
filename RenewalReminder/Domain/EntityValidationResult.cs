using System.ComponentModel.DataAnnotations;

namespace KvsProject.Domain
{
    public class EntityValidationResult
    {
        public IList<ValidationResult> ValidationErrors { get; private set; }

        public bool HasError
        {
            get { return ValidationErrors.Count > 0; }
        }

        public EntityValidationResult(IList<ValidationResult> violations)
        {
            ValidationErrors = violations;
            if (violations == null)
            {
                ValidationErrors = new List<ValidationResult>();
            }
        }

        public override string ToString()
        {
            return ToString(Environment.NewLine);
        }

        public string ToString(string seperator)
        {
            return string.Join(seperator, ValidationErrors.Where(a => !string.IsNullOrEmpty(a.ErrorMessage)).Select(a => a.ErrorMessage));
        }
    }
}

