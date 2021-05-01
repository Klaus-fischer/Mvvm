// <copyright file="ValidationResultErrorValidator{T}.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Validation
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Mvvm.Core;

    /// <summary>
    /// Implements <see cref="IDataErrorInfo"/> functionality to a model.
    /// </summary>
    /// <typeparam name="T">Type of model to validate.</typeparam>
    public abstract class ValidationResultErrorValidator<T> : ErrorValidator<T>
        where T : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResultErrorValidator{T}"/> class.
        /// </summary>
        /// <param name="model">Model to validate.</param>
        protected ValidationResultErrorValidator(T model)
            : base(model)
        {
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for;
        /// or null or System.String.Empty, to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        public override sealed IEnumerable GetErrors(string? propertyName)
             => this.GetAllErrors(propertyName);

        /// <summary>
        /// Validates the property and returns the error as string.
        /// </summary>
        /// <param name="propertyName">Property to validate.</param>
        /// <returns>All errors of this property as string.</returns>
        protected abstract IEnumerable<ValidationResult> GetPropertyErrors(string propertyName);

        private IEnumerable<ValidationResult> GetAllErrors(string? propertyName)
        {
            if (propertyName is null || propertyName.Trim() == string.Empty)
            {
                return this.PropertyNames.SelectMany(pn => this.GetAllErrors(pn));
            }

            var validationAttributeErrors = this.GetValidationAttributeErrors(propertyName);
            return this.GetPropertyErrors(propertyName).Concat(validationAttributeErrors);
        }

        private IEnumerable<ValidationResult> GetValidationAttributeErrors(string propertyName)
        {
            if (!this.UseValidationAttributes)
            {
                yield break;
            }

            PropertyInfo propertyInfo = this.Properties.FirstOrDefault(o => o.Name == propertyName)
                    ?? typeof(T).GetProperty(propertyName);

            if (propertyInfo is null)
            {
                yield break;
            }

            var attributes = propertyInfo.GetCustomAttributes(false)
                    .OfType<ValidationAttribute>()
                    .ToArray();

            foreach (var valAttr in attributes)
            {
                var value = propertyInfo.GetValue(this.Model);
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
                        yield return result;
                    }
                }
            }
        }
    }
}
