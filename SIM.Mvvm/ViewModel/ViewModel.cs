﻿// <copyright file="ViewModel.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The base view model.
    /// </summary>
    public abstract class ViewModel : IViewModel
    {
        private readonly Dictionary<string, object> supressedProperties = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            this.RegisterDependencies();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        void IViewModel.OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.OnPropertyChanged(propertyName, null, null);
        }

        /// <summary>
        /// ViewModel will suppress <see cref="INotifyPropertyChanged"/> notifications on the property after calling this method.
        /// </summary>
        /// <param name="propertyName">Name of the property to suppress notifications.</param>
        /// <param name="currentValue">The current value of the property.</param>
        public void SuppressNotifications(string propertyName, object currentValue)
        {
            if (!this.supressedProperties.ContainsKey(propertyName))
            {
                this.supressedProperties.Add(propertyName, currentValue);
            }
        }

        /// <summary>
        /// ViewModel will restore the <see cref="INotifyPropertyChanged"/> notifications on the properties.
        /// </summary>
        /// <param name="propertyName">Name of the property to restore notifications.</param>
        /// <param name="currentValue">The current value, to invoke <see cref="INotifyPropertyChanged"/> if values changed.</param>
        public void RestoreNotifications(string propertyName, object currentValue)
        {
            if (this.supressedProperties.TryGetValue(propertyName, out var oldValue))
            {
                this.supressedProperties.Remove(propertyName);

                if (!Equals(oldValue, currentValue))
                {
                    this.OnPropertyChanged(propertyName, oldValue, currentValue);
                }
            }
        }

        /// <summary>
        /// This call raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that get changed.</param>
        /// <param name="before">Old value of the property.</param>
        /// <param name="after">New value of the property.</param>
        protected void OnPropertyChanged(string? propertyName, object? before, object? after)
        {
            if (propertyName is null || this.supressedProperties.ContainsKey(propertyName))
            {
                return;
            }

            var args = new AdvancedPropertyChangedEventArgs(propertyName, before, after);
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
        /// <param name="comparer">Optional comparer to validate was changed.</param>
        /// <param name="propertyName">The name of the property that was changed.</param>
        /// <example>
        /// private int _property;
        /// public int Property
        /// {
        ///     get => this._property;
        ///     set => this.SetPropertyValue(ref _property, value);
        /// }
        /// ...
        /// </example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetPropertyValue<T>(
            ref T property,
            T newValue,
            IEqualityComparer<T>? comparer = null,
            [CallerMemberName] string propertyName = "")
        {
            if (!Equals(property, newValue, comparer))
            {
                T? oldValue = property;
                property = newValue;
                this.OnPropertyChanged(propertyName, oldValue, newValue);
            }
        }

        /// <summary>
        /// Compares the new and the old value.
        /// If values are different, the <see cref="OnPropertyChanged"/> method will be called.
        /// The return value is always the new property.
        /// </summary>
        /// <typeparam name="T">Type of the Property.</typeparam>
        /// <param name="expression">The expression that maps to the current value.</param>
        /// <param name="newValue">The value from the setter.</param>
        /// <param name="comparer">Optional comparer to validate was changed.</param>
        /// <param name="propertyName">The name of the property that was changed.</param>
        /// <example>
        /// private Model _propertyModel;
        /// public int Property
        /// {
        ///     get => this._propertyModel.Property;
        ///     set => this.SetPropertyValue(() => this._propertyModel.Property, value);
        /// }
        /// ...
        /// </example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetPropertyValue<T>(
            Expression<Func<T>> expression,
            T newValue,
            IEqualityComparer<T>? comparer = null,
            [CallerMemberName] string propertyName = "")
        {
            T oldValue = expression.Compile().Invoke();

            if (!Equals(oldValue, newValue, comparer))
            {
                if (expression.Body is MemberExpression me)
                {
                    var value = Expression.Constant(newValue);
                    var assign = Expression.Assign(me, value);
                    var lambda = Expression.Lambda<Action>(assign);
                    lambda.Compile().Invoke();
                }
                else
                {
                    throw new InvalidOperationException("Expression should point direct to target property. '() => this.Data.Property'");
                }

                this.OnPropertyChanged(propertyName, oldValue, newValue);
            }
        }

        private static bool Equals<T>(T property, T newValue, IEqualityComparer<T>? comparer)
        {
            // true if property and newValue is null
            if (ReferenceEquals(property, newValue))
            {
                return true;
            }

            // true if property or newValue is null
            if (property is null || newValue is null)
            {
                return false;
            }

            // try to use the comparer to validate equality.
            if (comparer?.Equals(property, newValue) ?? false)
            {
                return true;
            }

            // true if both values are equal.
            return property.Equals(newValue);
        }
    }
}
