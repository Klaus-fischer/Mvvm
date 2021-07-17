namespace Mvvm.VisualTest
{
    using Mvvm.Core;
    using Mvvm.Validation;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class SimpleErrorViewModel : ViewModel, INotifyDataErrorInfo
    {
        private readonly IValidator validator;

        public SimpleErrorViewModel()
        {
            this.validator = new ValidationResultErrorViewModelValidator(this)
            {
                ValidateOnPropertyChanged = true,
                UseValidationAttributes = true,
            };

            this.DataObject.PropertyChanged += this.DataObject_PropertyChanged;

            var context = new AsyncExecutionContext();
            Cancel = context.Cancel;
            Cmd = new AsyncRelayCommand(context, OnCmd, CanCmd);
        }

        public ICommand Cancel { get; }

        public ICommand Cmd { get; }

        private bool CanCmd() => true;

        private async Task OnCmd(CancellationToken token)
        {
            await Task.Delay(1000, token);
        }

        private void DataObject_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName, null, null);
        }

        public Data DataObject { get; set; } = new Data();

        public int Herkunft
        {
            get => this.DataObject.Herkunft;
            set => this.SetPropertyValue(() => this.DataObject.Herkunft, value);
        }

        public int Alter
        {
            get => this.DataObject.Alter;
            set => this.SetPropertyValue(() => this.DataObject.Alter, value);
        }

        public string Username { get; set; } = string.Empty;

        public string Test
        {
            get => field;
            set => this.SetPropertyValue(ref field, value);
        }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DisplayName("Passwort")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "{0} must have at least 8 and less than 12 character.")]
        public string Password { get; set; } = string.Empty;

        [NoValidation]
        public bool HasErrors => this.validator.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged
        {
            add => this.validator.ErrorsChanged += value;
            remove => this.validator.ErrorsChanged -= value;
        }

        public IEnumerable GetErrors(string? propertyName)
            => this.validator.GetErrors(propertyName);

        public bool Validate() => this.validator.Validate();

    }
}
