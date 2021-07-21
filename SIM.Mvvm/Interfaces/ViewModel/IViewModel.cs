﻿// <copyright file="IViewModel.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Declaration of an view model.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        event EventHandler<AdvancedPropertyChangedEventArgs>? AdvancedPropertyChanged;

        /// <summary>
        /// Gets the <see cref="IPropertyMonitor"/> of a property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>The property monitor to register callbacks and commands.</returns>
        public IPropertyMonitor this[string name] { get; }

        /// <summary>
        /// Gets the a collection of <see cref="IPropertyMonitor"/> for each property.
        /// </summary>
        /// <param name="names">The collection of names of the properties.</param>
        /// <returns>The property monitor to register callbacks and commands.</returns>
        public IEnumerable<IPropertyMonitor> this[params string[] names] { get; }

        /// <summary>
        /// To invoke the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that was changed.</param>
        void InvokeOnPropertyChanged(string propertyName);

        /// <summary>
        /// ViewModel will suppress <see cref="INotifyPropertyChanged"/> notifications on the property after calling this method.
        /// </summary>
        /// <param name="propertyName">Name of the property to suppress notifications.</param>
        /// <param name="currentValue">The current value of the property.</param>
        void SuppressNotifications(string propertyName, object currentValue);

        /// <summary>
        /// ViewModel will restore the <see cref="INotifyPropertyChanged"/> notifications on the properties.
        /// </summary>
        /// <param name="propertyName">Name of the property to restore notifications.</param>
        /// <param name="currentValue">The current value, to invoke <see cref="INotifyPropertyChanged"/> if values changed.</param>
        void RestoreNotifications(string propertyName, object currentValue);
    }
}
