// <copyright file="ICommandInvokeCanExecuteChangedEvent.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Interface to raise <see cref="ICommand.CanExecuteChanged"/> event.
    /// </summary>
    public interface ICommandInvokeCanExecuteChangedEvent : ICommand
    {
        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="sender">Sender of the event, must be the command itself.</param>
        /// <param name="args">EventArgs to submit.</param>
        void InvokeCanExecuteChanged(object sender, EventArgs args);
    }
}
