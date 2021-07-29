// <copyright file="ExpressionCollectionExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows.Input;
    using SIM.Mvvm;

    /// <summary>
    /// Extends <see cref="ExpressionExtensions"/> by collection calls.
    /// </summary>
    public static class ExpressionCollectionExtensions
    {
        /// <summary>
        /// Returns the result of <see cref="ExpressionExtensions.Listen{TProperty}(INotifyPropertyChanged, Expression{Func{TProperty}})"/> for all expressions..
        /// </summary>
        /// <param name="target">This property is only to extend the <see cref="IViewModel"/> class itself.</param>
        /// <param name="expressions">The collection of expression.</param>
        /// <returns>The collection of <see cref="IPropertyMonitor"/>.</returns>
        public static IEnumerable<IPropertyMonitor> Listen(
            this INotifyPropertyChanged target,
            params Expression<Func<object>>[] expressions)
        {
            foreach (var expression in expressions)
            {
                var exp = SingleExpressionCollectionExtensions.ConvertExpression(expression, out var propertyType);

                if (SingleExpressionCollectionExtensions.GetGenericMethodInfo(nameof(ExpressionExtensions.Listen), new Type[] { propertyType }) is MethodInfo methodInfo)
                {
                    yield return (IPropertyMonitor)methodInfo.Invoke(null, new object[] { target, exp });
                }
            }
        }

        /// <summary>
        /// Adds an parameterless callback for a property changed event.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        /// public string MySecondProperty { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty, () => this.MySecondProperty) // all properties to listen to
        ///         .Call(MyPropertyChanged, SomethingChanged); // all callbacks to register for each property changed event.
        /// }
        ///
        /// private void MyPropertyChanged()
        /// {
        ///     ...
        /// }
        ///
        /// private void SomethingChanged()
        /// {
        ///     ...
        /// }
        /// ...
        /// </example>
        /// <param name="monitors">The collection of property monitors to register the callbacks to.</param>
        /// <param name="action">The callback action.</param>
        /// <param name="additionalActions">Some additional actions to register.</param>
        /// <returns>The property monitor for chaining.</returns>
        public static IEnumerable<IPropertyMonitor> Call(
            this IEnumerable<IPropertyMonitor> monitors,
            Action action,
            params Action[] additionalActions)
        {
            foreach (var monitor in monitors)
            {
                monitor.Call(action, additionalActions);
            }

            return monitors;
        }

        /// <summary>
        /// Adds an <see cref="EventHandler{TEventArgs}"/> callback for a property changed event.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        /// public string MySecondProperty { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty, () => this.MySecondProperty) // all properties to listen to
        ///         .Call(this.MyPropertyChanged, this.SomePropertyChanged);    // all callbacks to register for each property changed event.
        /// }
        ///
        /// private void MyPropertyChanged(object sender, <see cref="AdvancedPropertyChangedEventArgs"/> arguments)
        /// {
        ///     ...
        /// }
        ///
        /// private void SomePropertyChanged(object sender, <see cref="AdvancedPropertyChangedEventArgs"/> arguments)
        /// {
        ///     ...
        /// }
        /// ...
        /// </example>
        /// <param name="monitors">The collection of property monitors to register the callbacks to.</param>
        /// <param name="eventHandler">The callback event handler.</param>
        /// <param name="additionalEventHandler">Some additional event handler.</param>
        /// <returns>The property monitor for chaining.</returns>
        public static IEnumerable<IPropertyMonitor> Call(
            this IEnumerable<IPropertyMonitor> monitors,
            EventHandler<AdvancedPropertyChangedEventArgs> eventHandler,
            params EventHandler<AdvancedPropertyChangedEventArgs>[] additionalEventHandler)
        {
            foreach (var monitor in monitors)
            {
                monitor.Call(eventHandler, additionalEventHandler);
            }

            return monitors;
        }

        /// <summary>
        /// Adds a property notification to a different view model.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        /// public string MySecondProperty { get; set; }
        ///
        /// public string SomeOtherProperty => this.MyProperty + "Something" + this.MySecondProperty;
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty, () => this.MySecondProperty)
        ///         .Notify(() => this.SomeOtherProperty);
        /// }
        /// ...
        /// </example>
        /// <param name="monitors">The collection of property monitors to register the callbacks to.</param>
        /// <param name="expressions">The collection of expression to the property to notify.
        /// Must be a MemberExpression to a <see cref="IViewModel"/> object.</param>
        /// <returns>The collection of property monitors.</returns>
        public static IEnumerable<IPropertyMonitor> Notify(
            this IEnumerable<IPropertyMonitor> monitors,
            params Expression<Func<object>>[] expressions)
        {
            foreach (var monitor in monitors)
            {
                monitor.Notify(expressions);
            }

            return monitors;
        }
    }
}
