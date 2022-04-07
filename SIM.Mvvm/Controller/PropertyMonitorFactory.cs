// <copyright file="PropertyMonitorFactory.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Factory class for <see cref="IPropertyMonitor"/> creation.
    /// </summary>
    public class PropertyMonitorFactory
    {
        private static Lazy<PropertyMonitorFactory> current = new Lazy<PropertyMonitorFactory>();

        /// <summary>
        /// Gets access to a singleton <see cref="PropertyMonitorFactory"/>.
        /// </summary>
        public static PropertyMonitorFactory Current => current.Value;

        /// <summary>
        /// Creates an property monitor to a view model.
        /// </summary>
        /// <param name="target">The target view model.</param>
        /// <param name="propertyName">The name of the property to monitor.</param>
        /// <returns>The property monitor.</returns>
        public IPropertyMonitor GetPropertyMonitor(INotifyPropertyChanged target, string propertyName)
        {
            var monitor = new PropertyMonitor(target, propertyName);
            return monitor;
        }
    }
}
