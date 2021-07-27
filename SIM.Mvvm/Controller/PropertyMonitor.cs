// <copyright file="PropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Monitor to watch for a single property of a <see cref="IViewModel"/>.
    /// </summary>
    /// <typeparam name="T">Type of the property to monitor.</typeparam>
    [DebuggerDisplay("PropertyMonitor -> {this.PropertyName}")]
    internal class PropertyMonitor<T> : IPropertyMonitor
    {
        private readonly IEqualityComparer<T> equalityComparer;
        private readonly Func<T> getValue;
        private readonly WeakReference<INotifyPropertyChanged> viewModelReference;

        private readonly Collection<ICommandInvokeCanExecuteChangedEvent> commands =
            new Collection<ICommandInvokeCanExecuteChangedEvent>();

        private readonly Collection<ViewModelNotifier> notifiers =
            new Collection<ViewModelNotifier>();

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="OnPropertyChanged"/> is suppressed.
        /// </summary>
        private bool isSuppressed;

        private T oldValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMonitor{T}"/> class.
        /// </summary>
        /// <param name="viewModel">View model to listen to.</param>
        /// <param name="propertyName">The name of the property the monitor listens to.</param>
        /// <param name="getValue">Request to get the property value.</param>
        public PropertyMonitor(INotifyPropertyChanged viewModel, string propertyName, Func<T> getValue, IEqualityComparer<T>? equalityComparer = null)
        {
            this.PropertyName = propertyName;
            this.getValue = getValue;
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            this.oldValue = getValue();

            this.viewModelReference = new WeakReference<INotifyPropertyChanged>(viewModel, false);

            viewModel.PropertyChanged += this.OnViewModelPropertyChangedHandler;
        }

        /// <inheritdoc/>
        public event EventHandler<AdvancedPropertyChangedEventArgs>? OnPropertyChanged;

        /// <inheritdoc/>
        public event Action? OnPropertyChangedCallback;

        /// <inheritdoc/>
        public string PropertyName { get; }

        /// <inheritdoc/>
        public INotifyPropertyChanged? Target
        {
            get
            {
                if (this.viewModelReference.TryGetTarget(out var value))
                {
                    return value;
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public TCommand RegisterCommand<TCommand>(TCommand command)
            where TCommand : ICommandInvokeCanExecuteChangedEvent
        {
            this.commands.Add(command);
            return command;
        }

        /// <inheritdoc/>
        public void UnregisterCommand(ICommandInvokeCanExecuteChangedEvent command)
            => this.commands.Remove(command);

        /// <inheritdoc/>
        void IPropertyMonitor.RegisterViewModelProperty(IViewModel target, string property)
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
        void IPropertyMonitor.UnregisterViewModelProperty(IViewModel target, string property)
        {
            if (this.notifiers.FirstOrDefault(o => o.CheckViewModel(target)) is ViewModelNotifier notifier)
            {
                notifier.RemoveProperty(property);
            }
        }

        /// <inheritdoc/>
        public void SuspendPropertyChanged()
        {
            this.isSuppressed = true;
        }

        /// <inheritdoc/>
        public void RestorePropertyChanged()
        {
            this.isSuppressed = false;

            this.OnViewModelPropertyChangedHandler(null, new PropertyChangedEventArgs(this.PropertyName));
        }

        private void OnViewModelPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != this.PropertyName || this.isSuppressed)
            {
                return;
            }

            var newValue = this.getValue();
            var oldValue = this.oldValue;

            this.oldValue = newValue;

            if (this.equalityComparer.Equals(oldValue, newValue))
            {
                return;
            }

            this.InvokeOnViewModelPropertyChanged(
                sender,
                new AdvancedPropertyChangedEventArgs(this.PropertyName, oldValue, newValue));
        }

        /// <summary>
        /// Invokes the <see cref="OnPropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event Arguments to submit.</param>
        private void InvokeOnViewModelPropertyChanged(object? sender, AdvancedPropertyChangedEventArgs e)
        {
            this.OnPropertyChanged?.Invoke(sender, e);

            this.OnPropertyChangedCallback?.Invoke();

            foreach (var command in this.commands)
            {
                command.InvokeCanExecuteChanged();
            }

            foreach (var notifier in this.notifiers)
            {
                notifier.InvokePropertyChanged();
            }
        }
    }
}
