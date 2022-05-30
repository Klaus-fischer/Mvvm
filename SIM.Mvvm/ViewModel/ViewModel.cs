// <copyright file="ViewModel.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The base view model.
    /// </summary>
    public abstract class ViewModel : IViewModel, IDisposable, IListenerHost
    {
        private readonly Collection<IPropertyListener> propertyListener = new();
        private readonly Dictionary<string, object?> supressedProperties = new();

        private bool isDisposed;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        void IViewModel.OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(propertyName, null, null);
        }

        /// <summary>
        /// Adds a property listener to the current view model.
        /// </summary>
        /// <param name="listener">Listener to register.</param>
        void IListenerHost.AddListener(IPropertyListener listener)
        {
            this.propertyListener.Add(listener);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;
            this.DisposeListener();

            this.OnDispose();
        }

        /// <summary>
        /// ViewModel will suppress <see cref="INotifyPropertyChanged"/> notifications on the property after calling this method.
        /// </summary>
        /// <param name="propertyName">Name of the property to suppress notifications.</param>
        /// <param name="currentValue">The current value of the property.</param>
        public void SuppressNotifications(string propertyName, object? currentValue = null)
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
        public void RestoreNotifications(string propertyName, object? currentValue = null)
        {
            if (this.supressedProperties.TryGetValue(propertyName, out var oldValue))
            {
                this.supressedProperties.Remove(propertyName);

                if (!this.Equals(propertyName, oldValue, currentValue))
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
            [CallerMemberName] string propertyName = "")
        {
            if (!this.Equals(propertyName, property, newValue))
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
            [CallerMemberName] string propertyName = "")
        {
            T oldValue = expression.Compile().Invoke();

            if (!this.Equals(propertyName, oldValue, newValue))
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

        /// <summary>
        /// Checks properties for equality.
        /// </summary>
        /// <typeparam name="T">Type of the property to compare.</typeparam>
        /// <param name="propertyName">The name of the property to compare for changes.</param>
        /// <param name="property">The current value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <returns>True if both are equal.</returns>
        protected virtual bool Equals<T>(string propertyName, T property, T newValue)
        {
            // also true if property and newValue is null
            if (ReferenceEquals(property, newValue))
            {
                return true;
            }

            // true if property or newValue is null
            if (property is null || newValue is null)
            {
                return false;
            }

            if (property is IEquatable<T> origin)
            {
                return origin.Equals(newValue);
            }

            // true if both values are equal.
            return property.Equals(newValue);
        }

        /// <summary>
        /// Will be executed on disposing.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        private void DisposeListener()
        {
            foreach (var listener in this.propertyListener)
            {
                if (listener is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            this.propertyListener.Clear();
        }
    }
}
