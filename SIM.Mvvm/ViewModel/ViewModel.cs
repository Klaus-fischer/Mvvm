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
    public abstract class ViewModel : IViewModel
    {
        internal static readonly string[] AllPropertyMontitorsToUnregister = { };

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            //this.RegisterDependencies();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        Collection<IPropertyMonitor> IViewModel.PropertyMonitors { get; }
            = new Collection<IPropertyMonitor>();

        /// <inheritdoc/>
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                property = newValue;
                this.OnPropertyChanged(propertyName);
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

                this.OnPropertyChanged(propertyName);
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

        void IViewModel.OnPropertyChanged(string propertyName)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// Register all property dependencies marked with the <see cref="DependsOnAttribute"/>,
        ///// to a <see cref="ICommand"/> that implements <see cref="ICommandInvokeCanExecuteChangedEvent"/>.
        ///// </summary>
        //private void RegisterDependencies()
        //{
        //    foreach (var property in this.GetType().GetProperties())
        //    {
        //        if (property.GetCustomAttribute<DependsOnAttribute>() is not DependsOnAttribute dependsOn)
        //        {
        //            continue;
        //        }

        //        if (typeof(ICommand).IsAssignableFrom(property.PropertyType))
        //        {
        //            // to Register an update command notification.
        //            _ = this.GetPropertyMonitor(property.Name);
        //        }

        //        this[dependsOn.PropertyNames].RegisterViewModelProperties(this, property.Name);
        //    }

        //    foreach (var method in this.GetType().GetMethods())
        //    {
        //        if (method.GetCustomAttribute<CallOnPropertyChangedAttribute>() is not CallOnPropertyChangedAttribute callOn)
        //        {
        //            continue;
        //        }

        //        if (!this.TryAssignCallback(method, callOn, out var message))
        //        {
        //            throw new InvalidOperationException($"Could not assign call back {message}");
        //        }
        //    }
        //}

        //private bool TryAssignCallback(MethodInfo method, CallOnPropertyChangedAttribute callOn, out string? errorMessage)
        //{
        //    var parameters = method.GetParameters();

        //    errorMessage = null;
        //    string prefix = $"{method.Name}({string.Join(", ", parameters.Select(o => o.ParameterType.Name))})\n";

        //    if (parameters.Length > 0)
        //    {
        //        if (parameters.Length != 2)
        //        {
        //            errorMessage = prefix + "Only zero or two parameters expected.";
        //        }
        //        else if (parameters[0].ParameterType != typeof(object))
        //        {
        //            errorMessage = prefix + "First parameter must have type 'object'";
        //        }
        //        else if (!typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType))
        //        {
        //            errorMessage = $"{prefix}Second parameter must be assignable to type '{nameof(AdvancedPropertyChangedEventArgs)}'";
        //        }
        //    }

        //    if (method.ReturnType != typeof(void))
        //    {
        //        errorMessage = $"Return type must be void";
        //    }

        //    if (errorMessage is not null)
        //    {
        //        return false;
        //    }

        //    if (parameters.Length == 0)
        //    {
        //        this[callOn.PropertyNames].RegisterCallback(
        //            (Action)method.CreateDelegate(typeof(Action), this));
        //    }
        //    else
        //    {
        //        this[callOn.PropertyNames].RegisterCallback(
        //            (EventHandler<AdvancedPropertyChangedEventArgs>)method.CreateDelegate(
        //                typeof(EventHandler<AdvancedPropertyChangedEventArgs>),
        //                this));
        //    }

        //    return true;
        //}
    }
}
