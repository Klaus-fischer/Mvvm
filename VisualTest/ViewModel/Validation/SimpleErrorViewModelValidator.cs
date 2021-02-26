namespace Mvvm.VisualTest
{
    using Mvvm.Core;
    using System.Collections.Generic;

    public class SimpleErrorViewModelValidator : StringErrorValidator<SimpleErrorViewModel>
    {
        public SimpleErrorViewModelValidator(SimpleErrorViewModel model)
            : base(model)
        {
        }

        protected override IEnumerable<string> GetPropertyErrors(string propertyName)
        {
            if (propertyName == nameof(SimpleErrorViewModel.Username))
            {
                if (this.Model.Username is null || this.Model.Username.Length < 5)
                {
                    yield return $"{propertyName} must have at least 5 character.";
                }
            }
        }
    }
}
