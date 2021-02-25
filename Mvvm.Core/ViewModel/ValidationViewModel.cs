// <copyright file="ValidationViewModel.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Implements <see cref="IDataErrorInfo"/> to the view model.
    /// </summary>
    public abstract class ValidationViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly IDictionary<string, bool> propertyHasErrors = new Dictionary<string, bool>();
        private IEnumerable<PropertyInfo>? properties;
        private bool validateOnPropertyChanged;

        /// <inheritdoc/>
        public event System.EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <inheritdoc/>
        [NoValidation]
        public bool HasErrors => this.propertyHasErrors.Values.Any();

        /// <summary>
        /// Gets or sets a value indicating whether properties gets validated on <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        [NoValidation]
        protected bool ValidateOnPropertyChanged
        {
            get => this.validateOnPropertyChanged;
            set
            {
                if (value)
                {
                    this.PropertyChanged += this.ValidateChangedProperty;
                }
                else
                {
                    this.PropertyChanged -= this.ValidateChangedProperty;
                }

                this.validateOnPropertyChanged = value;
            }
        }

        /// <summary>
        /// Gets the names of all public properties.
        /// </summary>
        [NoValidation]
        protected IEnumerable<string> PropertyNames
        {
            get
            {
                if (this.properties is null)
                {
                    this.properties = this.GetType()
                        .GetProperties()
                        .Where(o => o.GetCustomAttribute<NoValidationAttribute>() == null)
                        .ToArray();
                }

                return this.properties.Select(o => o.Name);
            }
        }

        /// <summary>
        /// This method will validate all public properties.
        /// </summary>
        /// <returns>True if any error.</returns>
        public bool Validate()
        {
            foreach (var propertyName in this.PropertyNames)
            {
                this.ValidateProperty(propertyName);
            }

            return this.HasErrors;
        }

        /// <inheritdoc/>
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
            => this.GetAllErrors(propertyName);

        /// <summary>
        /// Validates the property and returns the error as string.
        /// </summary>
        /// <param name="propertyName">Property to validate.</param>
        /// <returns>All errors of this property as string.</returns>
        protected abstract IEnumerable<string> GetErrors(string propertyName);

        /// <summary>
        /// Checks for errors for the changed property.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ValidateChangedProperty(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyNames.Contains(e.PropertyName))
            {
                this.ValidateProperty(e.PropertyName);
            }
        }

        private void ValidateProperty(string propertyName)
        {
            // backup has errors value.
            var viewModelHadErrors = this.HasErrors;

            var propertyHasErrors = this.GetAllErrors(propertyName).Any();

            if (propertyHasErrors)
            {
                this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            else if (this.propertyHasErrors.TryGetValue(propertyName, out var propertyHadErrors))
            {
                // if old errors get cleared.
                if (propertyHadErrors)
                {
                    this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }
            }

            this.propertyHasErrors[propertyName] = propertyHasErrors;

            // notify if property was changed.
            if (viewModelHadErrors != this.HasErrors)
            {
                this.OnPropertyChanged(nameof(this.HasErrors), viewModelHadErrors, this.HasErrors);
            }
        }

        private IEnumerable<string> GetAllErrors(string propertyName)
        {
            var validationAttributeErrors = this.GetValidationAttributeErrors(propertyName);
            return this.GetErrors(propertyName).Concat(validationAttributeErrors);
        }

        private IEnumerable<string> GetValidationAttributeErrors(string propertyName)
        {
            PropertyInfo? propertyInfo = this.properties.FirstOrDefault(o => o.Name == propertyName)
                ?? this.GetType().GetProperty(propertyName);

            if (propertyInfo is not null)
            {
                var attributes = propertyInfo.GetCustomAttributes(false)
                    .OfType<ValidationAttribute>()
                    .ToArray();

                foreach (var valAttr in attributes)
                {
                    var value = propertyInfo.GetValue(this);
                    var displayName = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
                    var context = new ValidationContext(this)
                    {
                        MemberName = propertyName,
                        DisplayName = displayName?.DisplayName ?? propertyName,
                    };

                    var results = new List<ValidationResult>();

                    if (!Validator.TryValidateValue(value, context, results, attributes))
                    {
                        foreach (var result in results)
                        {
                            yield return result.ErrorMessage;
                        }
                    }
                }
            }
        }
    }
}
