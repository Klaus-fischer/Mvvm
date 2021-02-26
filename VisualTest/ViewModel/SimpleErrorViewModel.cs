namespace Mvvm.VisualTest
{
    using Mvvm.Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SimpleErrorViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly IValidator validator;

        public SimpleErrorViewModel()
        {
            this.validator = new ValidationResultErrorViewModelValidator(this)
            {
                ValidateOnPropertyChanged = true,
                UseValidationAttributes = true,
            };
        }

        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DisplayName("Passwort")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "{0} must have at least 8 and less than 12 character.")]
        public string Password { get; set; }

        [NoValidation]
        public bool HasErrors => this.validator.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => this.validator.ErrorsChanged += value;
            remove => this.validator.ErrorsChanged -= value;
        }

        public IEnumerable GetErrors(string propertyName)
            => this.validator.GetErrors(propertyName);

        public bool Validate() => this.validator.Validate();
    }
}
