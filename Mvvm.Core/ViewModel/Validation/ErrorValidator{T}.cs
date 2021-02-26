// <copyright file="StringErrorValidator{T}.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Implements <see cref="IDataErrorInfo"/> functionality to a model.
    /// </summary>
    /// <typeparam name="T">Type of model to validate.</typeparam>
    public abstract class ErrorValidator<T> : IValidator
        where T : IViewModel
    {
        private readonly IDictionary<string, bool> propertyHasErrors = new Dictionary<string, bool>();
        private bool validateOnPropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorValidator{T}"/> class.
        /// </summary>
        /// <param name="model">Model to validate.</param>
        protected ErrorValidator(T model)
        {
            this.Model = model;
            this.Properties = typeof(T)
                .GetProperties()
                .Where(o => o.GetCustomAttribute<NoValidationAttribute>() == null)
                .ToArray();
        }

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Gets the model to validate.
        /// </summary>
        public T Model { get; }

        /// <summary>
        /// Gets a value indicating whether the entity has validation errors.
        /// true if the entity currently has validation errors; otherwise, false.
        /// </summary>
        public bool HasErrors => this.propertyHasErrors.Values.Any();

        /// <summary>
        /// Gets or sets a value indicating whether properties gets validated on <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        public bool ValidateOnPropertyChanged
        {
            get => this.validateOnPropertyChanged;
            set
            {
                if (value)
                {
                    this.Model.PropertyChanged += this.ValidateChangedProperty;
                }
                else
                {
                    this.Model.PropertyChanged -= this.ValidateChangedProperty;
                }

                this.validateOnPropertyChanged = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="ValidationAttribute"/> should be validated.
        /// </summary>
        public bool UseValidationAttributes { get; set; }

        /// <summary>
        /// Gets the collection of properties to observe.
        /// </summary>
        protected IEnumerable<PropertyInfo> Properties { get; }

        /// <summary>
        /// Gets the names of all public properties.
        /// </summary>
        protected IEnumerable<string> PropertyNames => this.Properties.Select(o => o.Name);

        /// <summary>
        /// This method will validate all public properties.
        /// </summary>
        /// <returns>False if any error.</returns>
        public virtual bool Validate()
        {
            foreach (var propertyName in this.PropertyNames)
            {
                this.ValidateProperty(propertyName);
            }

            return !this.HasErrors;
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for;
        /// or null or System.String.Empty, to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        public abstract IEnumerable GetErrors(string? propertyName);

        /// <summary>
        /// Invokes <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Property that error state was changed.</param>
        protected void InvokeErrorsChanged(string propertyName)
        {
            this.ErrorsChanged?.Invoke(this.Model, new DataErrorsChangedEventArgs(propertyName));
        }

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

            var propertyHasErrors = this.GetErrors(propertyName).OfType<object>().Any();

            if (propertyHasErrors)
            {
                this.InvokeErrorsChanged(propertyName);
            }
            else if (this.propertyHasErrors.TryGetValue(propertyName, out var propertyHadErrors))
            {
                // if old errors get cleared.
                if (propertyHadErrors)
                {
                    this.InvokeErrorsChanged(propertyName);
                }
            }

            this.propertyHasErrors[propertyName] = propertyHasErrors;

            // notify if property was changed.
            if (viewModelHadErrors != this.HasErrors)
            {
                this.Model.InvokeOnPropertyChanged(nameof(this.HasErrors));
            }
        }
    }
}
