// <copyright file="PropertyMonitorFactory.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Factory class for <see cref="IPropertyMonitor"/> creation.
    /// </summary>
    public class PropertyMonitorFactory
    {
        private static Lazy<PropertyMonitorFactory> current = new Lazy<PropertyMonitorFactory>();

        private readonly List<WeakReference<IPropertyMonitor>> weakPropertyMonitors
            = new List<WeakReference<IPropertyMonitor>>();

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
            // cleanup monitor collection
            this.weakPropertyMonitors.RemoveAll(o => !o.TryGetTarget(out var _));

            var monitor = this.weakPropertyMonitors
                .Select(o =>
                {
                    o.TryGetTarget(out var t);
                    return t;
                })
                .FirstOrDefault(m => m.PropertyName == propertyName &&
                                     m.Target == target);

            if (monitor is null)
            {
                monitor = new PropertyMonitor(target, propertyName);
                this.weakPropertyMonitors.Add(new WeakReference<IPropertyMonitor>(monitor));
            }

            return monitor;
        }
    }
}
