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
        /// <see cref="Listen{TProperty}(INotifyPropertyChanged, Expression{Func{TProperty}})"/>.
        /// </summary>
        /// <param name="target">This property is only to extend the <see cref="IViewModel"/> class itself.</param>
        /// <param name="expression">The anonym expression to the property.</param>
        /// <returns>The property monitor.</returns>
        internal static IPropertyMonitor Listen(this INotifyPropertyChanged target, Expression<Func<object>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess && expression.Body is MemberExpression me)
            {
                var propertyName = me.Member.Name;
                var obj = Expression.Lambda(me.Expression).Compile().DynamicInvoke();

                if (obj is INotifyPropertyChanged targetViewModel)
                {
                    return targetViewModel.GetPropertyMonitor(propertyName, expression);
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
        /// <param name="monitor">The property monitor to register the notification to.</param>
        /// <param name="expression">The Expression to the property to notify.
        /// Must be a MemberExpression to a <see cref="IViewModel"/> object.</param>
        /// <returns>The property monitor.</returns>
        public static IPropertyMonitor Notify(this IPropertyMonitor monitor, Expression<Func<object>> expression)
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
        ///     this.MyCommand = new EventCommand();    // or some other command that implements <see cref="ICommandInvokeCanExecuteChangedEvent"/>.
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
            if (command is ICommandInvokeCanExecuteChangedEvent cmd)
            {
                monitor.RegisterCommand(cmd);
                return monitor;
            }

            throw new InvalidOperationException($"Command must implement {nameof(ICommandInvokeCanExecuteChangedEvent)} interface.");
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
        ///     this.MyCommand = new EventCommand()    // or some other command that implements <see cref="ICommandInvokeCanExecuteChangedEvent"/>.
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
        public static T Listen<T>(this T command, Expression<Func<object>> expression)
            where T : ICommandInvokeCanExecuteChangedEvent
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess && expression.Body is MemberExpression me)
            {
                var propertyName = me.Member.Name;
                var obj = Expression.Lambda(me.Expression).Compile().DynamicInvoke();

                if (obj is INotifyPropertyChanged target)
                {
                    var monitor = target.GetPropertyMonitor(propertyName, expression);
                    monitor.RegisterCommand(command);
                }
            }

            throw new InvalidOperationException($"Expression must point to an Property of an view model of type {nameof(INotifyPropertyChanged)}.");
        }

        private static Expression ConvertExpression(out Type foundType, Expression<Func<object>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess && expression.Body is MemberExpression me)
            {
                foundType = me.Member.DeclaringType;

                var exp = Expression.MakeMemberAccess(me.Expression, foundType);
                return Expression.Lambda(exp);
            }

            throw new InvalidOperationException();
        }

        private static IPropertyMonitor GetPropertyMonitor(this INotifyPropertyChanged target, string propertyName, Expression<Func<object>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess && expression.Body is MemberExpression me)
            {
                var propertyType = me.Member.ReflectedType;

                var exp = Expression.MakeMemberAccess(me.Expression, propertyType);
                var lambda = Expression.Lambda(exp).Compile();

                var memberInfo = typeof(ExpressionExtensions).GetMethod($"{nameof(GetPropertyMonitor)}`1").MakeGenericMethod(propertyType);

                return (IPropertyMonitor)memberInfo.Invoke(null, new object[] { target, propertyName, lambda });
            }

            throw new InvalidOperationException();
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
