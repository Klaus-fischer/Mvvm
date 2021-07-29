// <copyright file="ExpressionExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Expressions
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Input;
    using SIM.Mvvm;

    /// <summary>
    /// Extends <see cref="IViewModel"/>, <see cref="ICommand"/> and <see cref="IPropertyMonitor"/> by Expression extensions.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Creates an <see cref="IPropertyMonitor"/> for the property inside the expression.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty).Call(MyPropertyChanged);
        /// }
        /// ...
        /// </example>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="target">This property is only to extend the <see cref="IViewModel"/> class itself.</param>
        /// <param name="expression">The Expression to the property.
        /// Must be a MemberExpression to a <see cref="INotifyPropertyChanged"/> object.</param>
        /// <returns>The property monitor.</returns>
        public static IPropertyMonitor Listen<TProperty>(this INotifyPropertyChanged target, Expression<Func<TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess && expression.Body is MemberExpression me)
            {
                var propertyName = me.Member.Name;
                var obj = Expression.Lambda(me.Expression).Compile().DynamicInvoke();

                if (obj is INotifyPropertyChanged targetViewModel)
                {
                    var factory = PropertyMonitorFactory.Current;

                    return factory.GetPropertyMonitor(targetViewModel, propertyName);
                }
            }

            throw new InvalidOperationException($"Expression must point to an Property of an view model of type {nameof(INotifyPropertyChanged)}.");
        }

        /// <summary>
        /// Adds an parameterless callback for a property changed event.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty).Call(MyPropertyChanged);
        /// }
        ///
        /// private void MyPropertyChanged()
        /// {
        ///     ...
        /// }
        /// ...
        /// </example>
        /// <param name="monitor">The property monitor to register the callback to.</param>
        /// <param name="action">The callback action.</param>
        /// <returns>The property monitor for chaining.</returns>
        public static IPropertyMonitor Call(this IPropertyMonitor monitor, Action action)
        {
            monitor.OnPropertyChangedCallback += action;
            return monitor;
        }

        /// <summary>
        /// Adds an <see cref="EventHandler{TEventArgs}"/> callback for a property changed event.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty).Call(MyPropertyChanged);
        /// }
        ///
        /// private void MyPropertyChanged(object sender, <see cref="AdvancedPropertyChangedEventArgs"/> arguments)
        /// {
        ///     ...
        /// }
        /// ...
        /// </example>
        /// <param name="monitor">The property monitor to register the callback to.</param>
        /// <param name="eventHandler">The callback event handler.</param>
        /// <returns>The property monitor for chaining.</returns>
        public static IPropertyMonitor Call(this IPropertyMonitor monitor, EventHandler<AdvancedPropertyChangedEventArgs> eventHandler)
        {
            monitor.OnPropertyChanged += eventHandler;
            return monitor;
        }

        /// <summary>
        /// Adds a property notification to a different view model.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        ///
        /// public string SomeOtherProperty => this.MyProperty + "Something";
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty).Notify(() => this.SomeOtherProperty);
        ///     //                  A                             A
        ///     //                  L  View Model to listen to    L  View Model to notify.
        /// }
        /// ...
        /// </example>
        /// <typeparam name="TProperty">Type of the target property.</typeparam>
        /// <param name="monitor">The property monitor to register the notification to.</param>
        /// <param name="expression">The Expression to the property to notify.
        /// Must be a MemberExpression to a <see cref="IViewModel"/> object.</param>
        /// <returns>The property monitor.</returns>
        public static IPropertyMonitor Notify<TProperty>(this IPropertyMonitor monitor, Expression<Func<TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess && expression.Body is MemberExpression me)
            {
                var propertyName = me.Member.Name;
                var obj = Expression.Lambda(me.Expression).Compile().DynamicInvoke();

                if (obj is IViewModel viewModel)
                {
                    monitor.RegisterViewModelProperty(viewModel, propertyName);

                    // check if is command
                    if (typeof(ICommand).IsAssignableFrom(typeof(TProperty)))
                    {
                        var factory = CommandNotifierFactory.Current;
                        var notifier = factory.GetPropertyMonitor(viewModel, propertyName);

                        // set callback if property changed.
                        monitor.Call(notifier.NotifyCommandChanged);
                    }

                    return monitor;
                }
            }

            throw new InvalidOperationException($"Expression must point to an Property of an view model of type {nameof(IViewModel)}.");
        }
    }
}
