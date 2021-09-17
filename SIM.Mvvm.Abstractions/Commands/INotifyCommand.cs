// <copyright file="INotifyCommand.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Interface to raise <see cref="ICommand.CanExecuteChanged"/> event.
    /// </summary>
    public interface INotifyCommand : ICommand
    {
        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        void NotifyCanExecuteChanged();
    }
}
