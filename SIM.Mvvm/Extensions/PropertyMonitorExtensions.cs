// <copyright file="ViewModelPropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class PropertyMonitorExtensions
    {
        public static T RegisterCallback<T>(this T monitor, Action callback)
            where T : IPropertyMonitor
        {
            monitor.OnViewModelPropertyChanged += (s, e) => callback();
            return monitor;
        }

        public static T RegisterCallback<T>(this T monitor, EventHandler<AdvancedPropertyChangedEventArgs> callback)
            where T : IPropertyMonitor
        {
            monitor.OnViewModelPropertyChanged += callback;
            return monitor;
        }

        public static T UnregisterCallback<T>(this T monitor, EventHandler<AdvancedPropertyChangedEventArgs> callback)
            where T : IPropertyMonitor
        {
            monitor.OnViewModelPropertyChanged -= callback;
            return monitor;
        }

        public static void RegisterCommands(
            this IPropertyMonitor monitor,
            params ICommandInvokeCanExecuteChangedEvent[] commands)
        {
            foreach (var command in commands)
            {
                monitor.RegisterCommand(command);
            }
        }

        public static void UnregisterCommands(
           this IPropertyMonitor monitor,
           params ICommandInvokeCanExecuteChangedEvent[] commands)
        {
            foreach (var command in commands)
            {
                monitor.UnregisterCommand(command);
            }
        }
    }
}
