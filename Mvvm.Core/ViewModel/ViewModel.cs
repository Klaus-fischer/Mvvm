// <copyright file="ViewModel.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    /// <summary>
    /// The base view model.
    /// </summary>
    public abstract class ViewModel : IViewModel
    {
        private readonly Dictionary<string, object> supressedProperties = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            this.RegisterDependencies();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<AdvancedPropertyChangedEventArgs>? AdvancedPropertyChanged;

        /// <inheritdoc/>
        void IViewModel.InvokeOnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(propertyName, null, null);
        }

        /// <inheritdoc/>
        public void SuppressNotifications(string propertyName, object currentValue)
        {
            if (!this.supressedProperties.ContainsKey(propertyName))
            {
                this.supressedProperties.Add(propertyName, currentValue);
            }
        }

        /// <inheritdoc/>
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
        /// This call raises the <see cref="AdvancedPropertyChanged"/> and <see cref="PropertyChanged"/> event.
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

        /// <summary>
        /// Register all property dependencies marked with the <see cref="DependsOnAttribute"/>,
        /// to a <see cref="ICommand"/> that implements <see cref="ICommandInvokeCanExecuteChangedEvent"/>.
        /// </summary>
        protected void RegisterDependencies()
        {
            foreach (var property in this.GetType().GetProperties())
            {
                if (property.GetCustomAttribute<DependsOnAttribute>() is not DependsOnAttribute dependsOn)
                {
                    continue;
                }

                if (typeof(ICommand).IsAssignableFrom(property.PropertyType))
                {
                    if (property.GetValue(this) is ICommandInvokeCanExecuteChangedEvent command)
                    {
                        command.RegisterPropertyDependency(this, dependsOn.PropertyNames);
                    }

                    _ = new ViewModelCommandMonitor(this, property.Name, dependsOn.PropertyNames);
                }
                else
                {
                    this.RegisterDependencies(this, property.Name, dependsOn.PropertyNames);
                }
            }
        }
    }
}
