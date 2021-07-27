// <copyright file="ExpressionCollectionExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
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
              //  yield return target.Listen(expression);
            }
            yield break;
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
                monitor.Call(action);

                foreach (var optional in additionalActions)
                {
                    monitor.Call(optional);
                }
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
                monitor.Call(eventHandler);

                foreach (var optional in additionalEventHandler)
                {
                    monitor.Call(optional);
                }
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
                foreach (var expression in expressions)
                {
                    //monitor.Notify(expression);
                }
            }

            return monitors;
        }

        /// <summary>
        /// Adds a <see cref="Command.CanExecuteChanged"/> callback to a view model.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        /// public string MySecondProperty { get; set; }
        ///
        /// public ICommand MyCommand { get; set; }
        /// public ICommand MySecondCommand { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.MyCommand = new EventCommand();          // or some other command that implements <see cref="INotifyCommand"/>.
        ///     this.MySecondCommand = new EventCommand();    // or some other command that implements <see cref="INotifyCommand"/>.
        ///
        ///     this.Listen(() => this.MyProperty, () => this.MySecondProperty)
        ///         .Notify(this.MyCommand, this.MySecondCommand);
        /// }
        /// ...
        /// </example>
        /// <param name="monitors">The collection of property monitors to register the callbacks to.</param>
        /// <param name="commands">The collection of commands to notify.
        /// Must be a MemberExpression to a <see cref="IViewModel"/> object.</param>
        /// <returns>The property monitor.</returns>
        public static IEnumerable<IPropertyMonitor> Notify(
            this IEnumerable<IPropertyMonitor> monitors,
            params ICommand[] commands)
        {
            foreach (var monitor in monitors)
            {
                foreach (var command in commands)
                {
                    monitor.Notify(command);
                }
            }

            return monitors;
        }

        /// <summary>
        /// Adds a <see cref="Command.CanExecuteChanged"/> callback to a view model.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        /// public string MySecondProperty { get; set; }
        ///
        /// public ICommand MyCommand { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.MyCommand = new EventCommand()
        ///         .Listen(() => this.MyProperty, () => this.MySecondProperty); // collection of properties to listen to.
        ///     //                  A       A
        ///     //                  |       L Property to listen to
        ///     //                  L must implement <see cref="INotifyPropertyChanged"/>.
        /// }
        /// ...
        /// </example>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <param name="command">The command to notify.</param>
        /// <param name="expressions">The collection of Expressions to the property.
        /// Must be a MemberExpression to a <see cref="INotifyPropertyChanged"/> object.</param>
        /// <returns>The property monitor.</returns>
        public static T Listen<T>(this T command, params Expression<Func<object>>[] expressions)
            where T : INotifyCommand
        {
            foreach (var expression in expressions)
            {
                command.Listen<T>(expression);
            }

            return command;
        }
    }
}
