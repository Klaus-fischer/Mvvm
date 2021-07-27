// <copyright file="ExpressionExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows.Input;
    using SIM.Mvvm;

    /// <summary>
    /// Extends <see cref="IViewModel"/>, <see cref="ICommand"/> and <see cref="IPropertyMonitor"/> by Expression extensions.
    /// </summary>
    public static class ExpressionExtensions
    {
        private static readonly Collection<WeakReference<IPropertyMonitor>> WeakPropertyMonitors
            = new Collection<WeakReference<IPropertyMonitor>>();

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
                    return targetViewModel.GetPropertyMonitor(propertyName, expression.Compile());
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
                    return monitor;
                }
            }

            throw new InvalidOperationException($"Expression must point to an Property of an view model of type {nameof(IViewModel)}.");
        }

        /// <summary>
        /// Adds a <see cref="Command.CanExecuteChanged"/> callback to a view model.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        ///
        /// public ICommand MyCommand { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.MyCommand = new EventCommand();    // or some other command that implements <see cref="INotifyCommand"/>.
        ///     this.Listen(() => this.MyProperty).Notify(this.MyCommand);
        /// }
        /// ...
        /// </example>
        /// <param name="monitor">The property monitor to register the notification to.</param>
        /// <param name="command">The command to notify.
        /// Must be a MemberExpression to a <see cref="IViewModel"/> object.</param>
        /// <returns>The property monitor.</returns>
        public static IPropertyMonitor Notify(this IPropertyMonitor monitor, ICommand command)
        {
            if (command is INotifyCommand cmd)
            {
                monitor.RegisterCommand(cmd);
                return monitor;
            }

            throw new InvalidOperationException($"Command must implement {nameof(INotifyCommand)} interface.");
        }

        /// <summary>
        /// Adds a <see cref="Command.CanExecuteChanged"/> callback to a view model.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        ///
        /// public ICommand MyCommand { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.MyCommand = new EventCommand()    // or some other command that implements <see cref="INotifyCommand"/>.
        ///         .Listen(() => this.MyProperty);
        ///     //                  A       A
        ///     //                  |       L Property to listen to
        ///     //                  L must implement <see cref="INotifyPropertyChanged"/>.
        /// }
        /// ...
        /// </example>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <typeparam name="TProperty">Type of the target property.</typeparam>
        /// <param name="command">The command to notify.</param>
        /// <param name="expression">The Expression to the property.
        /// Must be a MemberExpression to a <see cref="INotifyPropertyChanged"/> object.</param>
        /// <returns>The property monitor.</returns>
        public static T Listen<T, TProperty>(this T command, Expression<Func<TProperty>> expression)
            where T : INotifyCommand
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess && expression.Body is MemberExpression me)
            {
                var propertyName = me.Member.Name;

                var obj = Expression.Lambda(me.Expression).Compile().DynamicInvoke();

                if (obj is INotifyPropertyChanged target)
                {
                    var monitor = target.GetPropertyMonitor(propertyName, expression.Compile());
                    monitor.RegisterCommand(command);

                    return command;
                }
            }

            throw new InvalidOperationException($"Expression must point to an Property of an view model of type {nameof(INotifyPropertyChanged)}.");
        }

        private static IPropertyMonitor GetPropertyMonitor<TProperty>(this INotifyPropertyChanged target, string propertyName, Func<TProperty> expression)
        {
            if (target is IViewModel viewModel)
            {
                var monitor = viewModel.PropertyMonitors.FirstOrDefault(o => o.PropertyName == propertyName);
                if (monitor is IPropertyMonitor && ReferenceEquals(monitor.Target, target))
                {
                    return monitor;
                }

                monitor = new PropertyMonitor<TProperty>(target, propertyName, expression, null);
                viewModel.PropertyMonitors.Add(monitor);
                return monitor;
            }
            else
            {
                if (target.TryGetPropertyMonitor(propertyName, out var monitor))
                {
                    return monitor;
                }

                monitor = new PropertyMonitor<TProperty>(target, propertyName, expression, null);
                WeakPropertyMonitors.Add(new WeakReference<IPropertyMonitor>(monitor));
                return monitor;
            }
        }

        private static bool TryGetPropertyMonitor(this INotifyPropertyChanged target, string propertyName, out IPropertyMonitor monitor)
        {
            var obsoleteItems = new Collection<WeakReference<IPropertyMonitor>>();

            foreach (var item in WeakPropertyMonitors)
            {
                if (!item.TryGetTarget(out monitor))
                {
                    obsoleteItems.Add(item);
                    continue;
                }

                if (monitor.PropertyName == propertyName && monitor.Target == target)
                {
                    RemoveObsolete(obsoleteItems);
                    return true;
                }
            }

            monitor = null;

            RemoveObsolete(obsoleteItems);
            return false;
        }

        private static void RemoveObsolete(IEnumerable<WeakReference<IPropertyMonitor>> obsolete)
        {
            foreach (var item in obsolete)
            {
                WeakPropertyMonitors.Remove(item);
            }
        }
    }
}
