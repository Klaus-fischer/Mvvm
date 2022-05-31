// <copyright file="IPropertyListenerExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Linq.Expressions;
    using System.Windows.Input;

    /// <summary>
    /// Extension methods for all property listeners.
    /// </summary>
    public static class IPropertyListenerExtensions
    {
        /// <summary>
        /// Notifies other properties of a view model if listener raises event.
        /// </summary>
        /// <typeparam name="T">Type of the property to notify.</typeparam>
        /// <param name="listener">The listener to monitor.</param>
        /// <param name="expression">The expression of the notify target property.</param>
        /// <returns>The listener itself for chaining.</returns>
        /// <exception cref="InvalidOperationException">If Expression is not an member expression.</exception>
        public static IPropertyListener Notify<T>(this IPropertyListener listener, Expression<Func<T>> expression)
        {
            if (expression.Body.NodeType != ExpressionType.MemberAccess || !(expression.Body is MemberExpression me))
            {
                throw new InvalidOperationException($"Expression must point to an Property of an view model of type {nameof(IViewModel)}.");
            }

            var propertyName = me.Member.Name;
            var obj = Expression.Lambda(me.Expression).Compile().DynamicInvoke();
            bool success = false;

            if (obj is IViewModel viewModel)
            {
                listener.PropertyChanged += (s, a) =>
                {
                    viewModel.OnPropertyChanged(propertyName);
                };
                success = true;
            }

            if (typeof(ICommand).IsAssignableFrom(typeof(T)))
            {
                Func<T> getter = expression.Compile();
                listener.PropertyChanged += (s, a) =>
                {
                    if (getter() is INotifyCommand command)
                    {
                        command.NotifyCanExecuteChanged();
                    }
                };
                success = true;
            }

            if (!success)
            {
                throw new InvalidOperationException("Property does not belong to an IViewModel and is not a ICommand");
            }

            return listener;
        }

        /// <summary>
        /// Adds an callback to an listener.
        /// </summary>
        /// <param name="listener">Listener to extend.</param>
        /// <param name="action">The action to perform on property changed.</param>
        /// <returns>The listener itself for chaining.</returns>
        public static IPropertyListener Call(this IPropertyListener listener, Action action)
        {
            listener.PropertyChanged += (s, a) => action();
            return listener;
        }
    }
}
