// <copyright file="IPropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;

    /// <summary>
    /// The property listener declaration for explicit types.
    /// </summary>
    public interface IPropertyMonitor<T> : IPropertyListener
    {
        /// <summary>
        /// Notifies listener on source property changed.
        /// </summary>
        new event EventHandler<OnPropertyChangedEventArgs<T>>? PropertyChanged;

        /// <summary>
        /// Notifies a chaned listener, if instance gets disposed.
        /// </summary>
        event EventHandler? Disposed;
    }
}