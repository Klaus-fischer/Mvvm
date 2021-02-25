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
    public abstract class ValidationViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly IDictionary<string, string> errorMessages = new Dictionary<string, string>();
        private IEnumerable<PropertyInfo>? properties;
        private bool validateOnPropertyChanged;

        /// <inheritdoc/>
        [NoValidation]
        public string Error => string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether properties gets validated on <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
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

        /// <inheritdoc/>
        [NoValidation]
        public string this[string columnName]
        {
            get
            {
                if (this.errorMessages.TryGetValue(columnName, out var result))
                {
                    return result;
                }

                return string.Empty;
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
                this.errorMessages[propertyName] = this.ValidateProperty(propertyName);
            }

            return this.errorMessages.Values.All(string.IsNullOrWhiteSpace);
        }

        /// <summary>
        /// Validates a single property.
        /// <see cref="ValidationAttribute"/> annotations will be validated by default.
        /// </summary>
        /// <param name="propertyName">The property to validate.</param>
        /// <returns>string.Empty if valid.</returns>
        protected virtual string ValidateProperty(string propertyName)
        {
            PropertyInfo? propertyInfo = this.properties.FirstOrDefault(o => o.Name == propertyName)
                ?? this.GetType().GetProperty(propertyName);

            if (propertyInfo is not null)
            {
                var attributes = propertyInfo.GetCustomAttributes(false)
                    .OfType<ValidationAttribute>()
                    .ToArray();

                if (attributes.Any())
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
                        return results[0].ErrorMessage;
                    }
                }
            }

            return string.Empty;
        }

        private void ValidateChangedProperty(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyNames.Contains(e.PropertyName))
            {
                this.errorMessages[e.PropertyName] = this.ValidateProperty(e.PropertyName);
            }
        }
    }
}
