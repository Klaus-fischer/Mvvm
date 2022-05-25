// <copyright file="IListenerHostExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Extension methods for <see cref="IListenerHost"/>.
    /// </summary>
    public static class IListenerHostExtensions
    {
        /// <summary>
        /// Creates an listener for an view model and register it on the host.
        /// </summary>
        /// <typeparam name="T">Type of the view model.</typeparam>
        /// <typeparam name="TProperty">Type of the property to listen to.</typeparam>
        /// <param name="host">Host to register the listener to.</param>
        /// <param name="source">The view model to listen to.</param>
        /// <param name="propertyExpression">The Expression to the property.
        /// Must be a MemberExpression.</param>
        /// <returns>The property Listener.</returns>
        /// <exception cref="InvalidOperationException">If Expression is not an memberexpression.</exception>
        public static IPropertyListener<TProperty> Listen<T, TProperty>(this IListenerHost host, T source, Expression<Func<T, TProperty>> propertyExpression)
           where T : INotifyPropertyChanged
        {
            if (propertyExpression.Body.NodeType == ExpressionType.MemberAccess &&
                propertyExpression.Body is MemberExpression me)
            {
                var propertyName = me.Member.Name;
                var listener = new PropertyListener<T, TProperty>(source, propertyName, propertyExpression.Compile());
                host.AddListener(listener);

                return listener;
            }

            throw new InvalidOperationException($"Expression must point to an Property of an view model of type {nameof(INotifyPropertyChanged)}.");
        }

        /// <summary>
        /// Creates an listener for an view model and register it on the view model host.
        /// </summary>
        /// <typeparam name="T">Type of the view model.</typeparam>
        /// <typeparam name="TProperty">Type of the property to listen to.</typeparam>
        /// <param name="host">Source to listen and to register the listener to.</param>
        /// <param name="propertyExpression">The Expression to the property.
        /// Must be a MemberExpression.</param>
        /// <returns>The property Listener.</returns>
        /// <exception cref="InvalidOperationException">If Expression is not an memberexpression.</exception>
        public static IPropertyListener<TProperty> Listen<T, TProperty>(this T host, Expression<Func<T, TProperty>> propertyExpression)
            where T : IListenerHost, INotifyPropertyChanged
        {
            return host.Listen(host, propertyExpression);
        }
    }
}
