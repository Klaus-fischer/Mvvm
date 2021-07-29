// <copyright file="ICommandNotifier.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System.ComponentModel;

    /// <summary>
    /// Holds command structure to notify on dependency changed.
    /// </summary>
    public interface ICommandNotifier
    {
        /// <summary>
        /// Gets the name of the command property from the view model.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Gets the reference to the view model the command belongs to.
        /// </summary>
        INotifyPropertyChanged? Target { get; }

        /// <summary>
        /// Raises <see cref="INotifyCommand.NotifyCanExecuteChanged()"/>.
        /// </summary>
        void NotifyCommandChanged();
    }
}