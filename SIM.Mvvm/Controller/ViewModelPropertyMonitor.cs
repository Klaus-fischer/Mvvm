// <copyright file="ViewModelPropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Defines a monitor that listens to an single property of an view model.
    /// </summary>
    public interface IPropertyMonitor
    {
        /// <summary>
        /// Event will be forwarded from view model, if property name matches.
        /// </summary>
        public event EventHandler<AdvancedPropertyChangedEventArgs> OnViewModelPropertyChanged;

        /// <summary>
        /// To register a command dependency.
        /// </summary>
        /// <param name="command">Command to register.</param>
        public void RegisterCommand(ICommandInvokeCanExecuteChangedEvent command);

        /// <summary>
        /// Unregister command dependency.
        /// </summary>
        /// <param name="command">Command to unregister.</param>
        public void UnregisterCommand(ICommandInvokeCanExecuteChangedEvent command);

        /// <summary>
        /// Registers a dependent view model.
        /// </summary>
        /// <param name="target">View model to register if property was changed.</param>
        /// <param name="property">Name of the property that will be effected.</param>
        void RegisterViewModelProperty(IViewModel target, string property);

        /// <summary>
        /// Unregisters a dependent view model.
        /// </summary>
        /// <param name="target">View model to register if property was changed.</param>
        /// <param name="property">Name of the property that will be effected.</param>
        void UnregisterViewModelProperty(IViewModel target, string property);
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

        private Collection<ICommandInvokeCanExecuteChangedEvent> commands = new ();

        private Collection<ViewModelNotifier> notifiers = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMonitor"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property the monitor listens to.</param>
        public PropertyMonitor(string propertyName)
        {
            this.propertyName = propertyName;
        }

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

            foreach (var notifier in this.notifiers)
            {
                notifier.InvokePropertyChanged();
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

        /// <inheritdoc/>
        public void RegisterViewModelProperty(IViewModel target, string property)
        {
            var notifier = this.notifiers.FirstOrDefault(o => o.CheckViewModel(target));
            if (notifier is null)
            {
                notifier = new ViewModelNotifier(target);
                this.notifiers.Add(notifier);
            }

            notifier.AddProperty(property);
        }

        /// <inheritdoc/>
        public void UnregisterViewModelProperty(IViewModel target, string property)
        {
            var notifier = this.notifiers.FirstOrDefault(o => o.CheckViewModel(target));
            if (notifier is null)
            {
                return;
            }

            notifier.RemoveProperty(property);
        }
    }
}
