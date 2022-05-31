// <copyright file="IPropertyMonitorExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;

    /// <summary>
    /// Extension methods for all property listeners.
    /// </summary>
    public static class IPropertyMonitorExtensions
    {
        /// <summary>
        /// Adds an callback to an listener.
        /// </summary>
        /// <typeparam name="T">Type of the observed propert</typeparam>
        /// <param name="monitor">Listener to extend.</param>
        /// <param name="eventHandler">The event handler to register to the listener.</param>
        /// <returns>The listener itself for chaining.</returns>
        public static IPropertyListener Call<T>(
            this IPropertyMonitor<T> monitor,
            EventHandler<OnPropertyChangedEventArgs<T>> eventHandler)
        {
            monitor.PropertyChanged += eventHandler;
            return monitor;
        }

        /// <summary>
        /// Adds an condition to the listener, to filter all notifications.
        /// </summary>
        /// <typeparam name="T">Type of the property to notify.</typeparam>
        /// <param name="monitor">The listener to filter.</param>
        /// <param name="predicate">The filter condition.</param>
        /// <returns>The condition property listener.</returns>
        public static IPropertyMonitor<T> If<T>(this IPropertyMonitor<T> monitor, Func<T, bool> predicate)
        {
            return new PredicatePropertyMonitor<T>(monitor, predicate);
        }
    }
}
