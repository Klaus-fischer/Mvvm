// <copyright file="CommandNotifierFactory.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Factory class for <see cref="IPropertyMonitor"/> creation.
    /// </summary>
    public class CommandNotifierFactory
    {
        private static Lazy<CommandNotifierFactory> current = new Lazy<CommandNotifierFactory>();

        /// <summary>
        /// Gets access to a singleton <see cref="PropertyMonitorFactory"/>.
        /// </summary>
        public static CommandNotifierFactory Current => current.Value;

        /// <summary>
        /// Creates an property monitor.
        /// </summary>
        /// <param name="target">The target view model.</param>
        /// <param name="commandName">The name of the property to monitor.</param>
        /// <returns>The property monitor.</returns>
        public ICommandNotifier GetCommandNotifier(INotifyPropertyChanged target, string commandName)
        {
            var notifier = new CommandNotifier(target, commandName);
            return notifier;
        }
    }
}
