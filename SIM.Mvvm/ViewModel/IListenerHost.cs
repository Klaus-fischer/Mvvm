// <copyright file="IListenerHost.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    /// <summary>
    /// Defines a host of listener.
    /// </summary>
    public interface IListenerHost
    {
        /// <summary>
        /// Adds a property listener to the current view model.
        /// </summary>
        /// <param name="listener">Listener to register.</param>
        void AddListener(IPropertyListener listener);
    }
}