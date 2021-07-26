// <copyright file="PropertyMonitorExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;

    public static class PropertyMonitorExtensions
    {
        public static T RegisterCallback<T>(this T monitor, Action callback)
            where T : IPropertyMonitor
        {
            monitor.OnPropertyChangedCallback += callback;
            return monitor;
        }

        public static T UnregisterCallback<T>(this T monitor, Action callback)
           where T : IPropertyMonitor
        {
            monitor.OnPropertyChangedCallback -= callback;
            return monitor;
        }

        public static T RegisterCallback<T>(this T monitor, EventHandler<AdvancedPropertyChangedEventArgs> callback)
            where T : IPropertyMonitor
        {
            monitor.OnPropertyChanged += callback;
            return monitor;
        }

        public static T UnregisterCallback<T>(this T monitor, EventHandler<AdvancedPropertyChangedEventArgs> callback)
            where T : IPropertyMonitor
        {
            monitor.OnPropertyChanged -= callback;
            return monitor;
        }

        public static T RegisterCommands<T>(
            this T monitor,
            params ICommandInvokeCanExecuteChangedEvent[] commands)
            where T : IPropertyMonitor
        {
            foreach (var command in commands)
            {
                monitor.RegisterCommand(command);
            }

            return monitor;
        }

        public static T UnregisterCommands<T>(
            this T monitor,
            params ICommandInvokeCanExecuteChangedEvent[] commands)
            where T : IPropertyMonitor
        {
            foreach (var command in commands)
            {
                monitor.UnregisterCommand(command);
            }

            return monitor;
        }

        public static T RegisterViewModelProperties<T>(
            this T monitor,
            IViewModel target,
            params string[] propertyNames)
            where T : IPropertyMonitor
        {
            foreach (var name in propertyNames)
            {
                monitor.RegisterViewModelProperty(target, name);
            }

            return monitor;
        }

        public static T UnregisterViewModelProperties<T>(
            this T monitor,
            IViewModel target,
            params string[] propertyNames)
            where T : IPropertyMonitor
        {
            foreach (var name in propertyNames)
            {
                monitor.UnregisterViewModelProperty(target, name);
            }

            return monitor;
        }
    }
}
