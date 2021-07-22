// <copyright file="ViewModelPropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public interface IPropertyMonitor
    {
        public event EventHandler<AdvancedPropertyChangedEventArgs> OnViewModelPropertyChanged;

        public void RegisterCommand(ICommandInvokeCanExecuteChangedEvent command);

        public void UnregisterCommand(ICommandInvokeCanExecuteChangedEvent command);
    }

    /// <summary>
    /// Monitor to watch for a single property of a <see cref="IViewModel"/>.
    /// </summary>
    internal class PropertyMonitor : IPropertyMonitor
    {
        /// <summary>
        /// The name of the property the monitor listens to.
        /// </summary>
        private readonly string propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMonitor"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property the monitor listens to.</param>
        public PropertyMonitor(string propertyName)
        {
            this.propertyName = propertyName;
        }

        private Collection<ICommandInvokeCanExecuteChangedEvent> commands = new();

        private Collection<ViewModelNotifier> notifiers = new ();

        /// <inheritdoc/>
        public event EventHandler<AdvancedPropertyChangedEventArgs>? OnViewModelPropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="OnViewModelPropertyChanged"/> is suppressed.
        /// </summary>
        public bool IsSuppressed { get; set; }

        /// <summary>
        /// Gets or sets the value before the monitor was switched off by setting <see cref="IsSuppressed"/> to true.
        /// </summary>
        public object? StoredValue { get; set; }

        /// <summary>
        /// Invokes the <see cref="OnViewModelPropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event Arguments to submit.</param>
        public void InvokeOnViewModelPropertyChanged(object sender, AdvancedPropertyChangedEventArgs e)
        {
            if (!Equals(this.propertyName, e.PropertyName) || this.IsSuppressed)
            {
                return;
            }

            this.OnViewModelPropertyChanged?.Invoke(sender, e);
            foreach (var command in this.commands)
            {
                command.InvokeCanExecuteChanged();
            }
        }

        /// <inheritdoc/>
        public void RegisterCommand(ICommandInvokeCanExecuteChangedEvent command)
        {
            this.commands.Add(command ?? throw new ArgumentNullException(nameof(command)));
        }

        /// <inheritdoc/>
        public void UnregisterCommand(ICommandInvokeCanExecuteChangedEvent command)
        {
            this.commands.Remove(command ?? throw new ArgumentNullException(nameof(command)));
        }

        public void RegisterViewModelProperty(IViewModel target, string property)
        {
            var notifier = this.notifierls.FirstOrDefault(o => o.CheckViewModel(target));
            if (notifier is null)
            {
                notifier = new ViewModelNotifier(target);
                this.notifiers.Add(notifier);
            }
            
            notifier.AddProperty(property);
        }  

        public void UnregisterViewModelProperty(IViewModel target, string property)
        {
            var notifier = this.notifierls.FirstOrDefault(o => o.CheckViewModel(target));
            if (notifier is null)
            {
                return;
            }
            
            notifier.RemoveProperty(property);
        }          
    }

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
