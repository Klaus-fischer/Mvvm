// <copyright file="IPropertyListener.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;

    /// <summary>
    /// The property listener declaration.
    /// </summary>
    public interface IPropertyListener
    {
        /// <summary>
        /// Notifies listener on source property changed.
        /// </summary>
        event EventHandler? PropertyChanged;

        /// <summary>
        /// Gets the name of the property to listen to.
        /// </summary>
        string PropertyName { get; }
    }

    /// <summary>
    /// The property listener declaration for explicit types.
    /// </summary>
    public interface IPropertyListener<T> : IPropertyListener
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