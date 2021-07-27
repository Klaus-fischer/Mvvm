// <copyright file="IPropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines a monitor that listens to an single property of an view model.
    /// </summary>
    public interface IPropertyMonitor
    {
        /// <summary>
        /// Event will be raised, if target property changed.
        /// </summary>
        event EventHandler<AdvancedPropertyChangedEventArgs> OnPropertyChanged;

        /// <summary>
        /// Callback will be raised, if target property changed.
        /// </summary>
        event Action OnPropertyChangedCallback;

        /// <summary>
        /// Gets the name of the property that will be monitored.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Gets the reference to the target to observe.
        /// </summary>
        INotifyPropertyChanged? Target { get; }

        /// <summary>
        /// To register a command dependency.
        /// </summary>
        /// <param name="command">Command to register.</param>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <returns>The command itself for chaining.</returns>
        T RegisterCommand<T>(T command)
            where T : ICommandInvokeCanExecuteChangedEvent;

        /// <summary>
        /// Unregister command dependency.
        /// </summary>
        /// <param name="command">Command to unregister.</param>
        void UnregisterCommand(ICommandInvokeCanExecuteChangedEvent command);

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

        /// <summary>
        /// Suppresses all property changed notifications.
        /// </summary>
        void SuspendPropertyChanged();

        /// <summary>
        /// Restores all property changed notifications.
        /// notifications will be raised, if property was changed.
        /// </summary>
        void RestorePropertyChanged();
    }
}
