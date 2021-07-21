// <copyright file="StringErrorValidator{T}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace Sim.Mvvm.Validation
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using SIM.Mvvm;

    /// <summary>
    /// Implements <see cref="IDataErrorInfo"/> functionality to a model.
    /// </summary>
    /// <typeparam name="T">Type of model to validate.</typeparam>
    public abstract class StringErrorValidator<T> : ErrorValidator<T>
        where T : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringErrorValidator{T}"/> class.
        /// </summary>
        /// <param name="model">Model to validate.</param>
        protected StringErrorValidator(T model)
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
        protected abstract IEnumerable<string> GetPropertyErrors(string propertyName);

        private IEnumerable<string> GetAllErrors(string? propertyName)
        {
            if (propertyName is null || propertyName.Trim() == string.Empty)
            {
                return this.PropertyNames.SelectMany(pn => this.GetAllErrors(pn));
            }

            var validationAttributeErrors = this.GetValidationAttributeErrors(propertyName);
            return this.GetPropertyErrors(propertyName).Concat(validationAttributeErrors);
        }

        private IEnumerable<string> GetValidationAttributeErrors(string propertyName)
        {
            if (!this.UseValidationAttributes ||
                !this.TryGetPropertyByName(propertyName, out var propertyInfo) ||
                propertyInfo is null)
            {
                yield break;
            }

            var attributes = this.GetValidationAttributes(propertyInfo);

            foreach (var valAttr in attributes)
            {
                var value = propertyInfo.GetValue(this.Model);
                var displayName = this.GetDisplayNameAttribute(propertyInfo);
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
                        yield return result.ErrorMessage ?? string.Empty;
                    }
                }
            }
        }
    }
}
