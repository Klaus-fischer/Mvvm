namespace Mvvm.VisualTest
{
    using Mvvm.Core;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class ValidationResultErrorViewModelValidator : ValidationResultErrorValidator<SimpleErrorViewModel>
    {
        public ValidationResultErrorViewModelValidator(SimpleErrorViewModel model)
            : base(model)
        {
        }

        protected override IEnumerable<ValidationResult> GetPropertyErrors(string propertyName)
        {
            if (propertyName == nameof(SimpleErrorViewModel.Username))
            {
                if (this.Model.Username is null || this.Model.Username.Length < 5)
                {
                    yield return new ValidationResult($"{propertyName} must have at least 5 character.");
                }

                if (this.Model.Username is not null && this.Model.Username.Length > 10)
                {
                    yield return new HintValidationResult($"{propertyName} should have not more than 10 characters.");
                }
            }
        }

        public override bool Validate()
        {
            base.Validate();

            return !this.GetErrors(null)
                .OfType<ValidationResult>()
                .Any(o => o is not HintValidationResult);
        }
    }
}
