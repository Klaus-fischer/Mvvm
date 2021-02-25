// <copyright file="BaseViewModel.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The base view model.
    /// </summary>
    public abstract class BaseViewModel : IViewModel
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        protected event EventHandler<AdvancedPropertyChangedEventArgs>? AdvancedPropertyChanged;

        /// <inheritdoc/>
        void IViewModel.InvokeOnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(propertyName, null, null);
        }

        /// <summary>
        /// This call raises the <see cref="AdvancedPropertyChanged"/> and <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that get changed.</param>
        /// <param name="before">Old value of the property.</param>
        /// <param name="after">New value of the property.</param>
        protected void OnPropertyChanged(string propertyName, object? before, object? after)
        {
            var args = new AdvancedPropertyChangedEventArgs(propertyName, before, after);
            this.AdvancedPropertyChanged?.Invoke(this, args);
            this.PropertyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Compares the new and the old value.
        /// If values are different, the <see cref="OnPropertyChanged"/> method will be called.
        /// The return value is always the new property.
        /// </summary>
        /// <typeparam name="T">Type of the Property.</typeparam>
        /// <param name="property">The reference to the current value.</param>
        /// <param name="newValue">The value from the setter.</param>
        /// <param name="propertyName">The name of the property that was changed.</param>
        /// <example>
        /// private int _property;
        /// public int Property
        /// {
        ///     get => _property;
        ///     set => this.SetPropertyValue(ref _property, value);
        /// }
        /// ...
        /// </example>
        protected void SetPropertyValue<T>(
            ref T? property,
            T? newValue,
            [CallerMemberName] string propertyName = "")
        {
            var valueEquals = ReferenceEquals(property, newValue);
            valueEquals |= property is null ^ newValue is null;
            valueEquals |= property?.Equals(newValue) ?? false;

            if (!valueEquals)
            {
                T oldValue = property;
                property = newValue;
                this.OnPropertyChanged(propertyName, oldValue, newValue);
            }
        }
    }
}
