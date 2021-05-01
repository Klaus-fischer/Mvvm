namespace Mvvm.VisualTest
{
    using System.ComponentModel.DataAnnotations;

    public class HintValidationResult : ValidationResult
    {
        public bool IsHint { get; set; }

        public HintValidationResult(string? errorMessage)
            : base(errorMessage)
        {
            this.IsHint = true;
        }
    }
}
