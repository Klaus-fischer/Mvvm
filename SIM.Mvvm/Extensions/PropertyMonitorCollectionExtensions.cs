// <copyright file="ViewModelPropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class PropertyMonitorCollectionExtensions
    {
        public static IEnumerable<T> RegisterCallback<T>(this IEnumerable<T> monitorCollection, Action callback)
            where T : IPropertyMonitor
        {
            foreach (var monitor in monitorCollection)
            {
                monitor.OnViewModelPropertyChanged += (s, e) => callback();
            }

            return monitorCollection;
        }

        public static IEnumerable<T> RegisterCallback<T>(this IEnumerable<T> monitorCollection, EventHandler<AdvancedPropertyChangedEventArgs> callback)
            where T : IPropertyMonitor
        {
            foreach (var monitor in monitorCollection)
            {
                monitor.OnViewModelPropertyChanged += callback;
            }

            return monitorCollection;
        }

        public static IEnumerable<T> UnregisterCallback<T>(this IEnumerable<T> monitorCollection, EventHandler<AdvancedPropertyChangedEventArgs> callback)
            where T : IPropertyMonitor
        {
            foreach (var monitor in monitorCollection)
            {
                monitor.OnViewModelPropertyChanged += callback;
            }

            return monitorCollection;
        }

        public static IEnumerable<T> RegisterCommand<T>(this IEnumerable<T> monitorCollection, ICommandInvokeCanExecuteChangedEvent command)
             where T : IPropertyMonitor
        {
            foreach (var monitor in monitorCollection)
            {
                monitor.RegisterCommand(command);
            }

            return monitorCollection;
        }

        public static IEnumerable<T> UnregisterCommand<T>(this IEnumerable<T> monitorCollection, ICommandInvokeCanExecuteChangedEvent command)
            where T : IPropertyMonitor
        {
            foreach (var monitor in monitorCollection)
            {
                monitor.UnregisterCommand(command);
            }

            return monitorCollection;
        }

        public static IEnumerable<T> RegisterCommands<T>(this IEnumerable<T> monitorCollection, params ICommandInvokeCanExecuteChangedEvent[] commands)
             where T : IPropertyMonitor
        {
            foreach (var monitor in monitorCollection)
            {
                monitor.RegisterCommands(commands);
            }

            return monitorCollection;
        }

        public static IEnumerable<T> UnregisterCommands<T>(this IEnumerable<T> monitorCollection, params ICommandInvokeCanExecuteChangedEvent[] commands)
            where T : IPropertyMonitor
        {
            foreach (var monitor in monitorCollection)
            {
                monitor.UnregisterCommands(commands);
            }

            return monitorCollection;
        }
    }
}
