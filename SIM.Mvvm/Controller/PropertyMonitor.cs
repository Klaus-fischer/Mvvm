﻿// <copyright file="PropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// Monitor to watch for a single property of a <see cref="IViewModel"/>.
    /// </summary>
    [DebuggerDisplay("PropertyMonitor -> {this.PropertyName}")]
    internal class PropertyMonitor : IPropertyMonitor
    {
        private readonly List<ViewModelNotifier> notifiers
            = new List<ViewModelNotifier>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMonitor"/> class.
        /// </summary>
        /// <param name="viewModel">View model to listen to.</param>
        /// <param name="propertyName">The name of the property the monitor listens to.</param>
        public PropertyMonitor(INotifyPropertyChanged viewModel, string propertyName)
        {
            this.Target = viewModel;
            this.PropertyName = propertyName;
            viewModel.PropertyChanged += this.OnViewModelPropertyChangedHandler;
        }

        /// <inheritdoc/>
        public event EventHandler<AdvancedPropertyChangedEventArgs>? OnPropertyChanged;

        /// <inheritdoc/>
        public event Action? OnPropertyChangedCallback;

        /// <inheritdoc/>
        public string PropertyName { get; }

        /// <inheritdoc/>
        public INotifyPropertyChanged? Target { get; }

        /// <inheritdoc/>
        void IPropertyMonitor.RegisterViewModelProperty(IViewModel target, string property)
        {
            var notifier = this.notifiers.Find(o => o.CheckViewModel(target));
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
            if (this.notifiers.Find(o => o.CheckViewModel(target)) is ViewModelNotifier notifier)
            {
                notifier.RemoveProperty(property);
            }
        }

        private void OnViewModelPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != this.PropertyName)
            {
                return;
            }

            if (e is not AdvancedPropertyChangedEventArgs advancedEventArgs)
            {
                advancedEventArgs = new AdvancedPropertyChangedEventArgs(e.PropertyName, null, null);
            }

            this.InvokeOnViewModelPropertyChanged(sender, advancedEventArgs);
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

            foreach (var notifier in this.notifiers)
            {
                notifier.InvokePropertyChanged();
            }
        }
    }
}
