// <copyright file="ICommandInvokeCanExecuteChangedEvent.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
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
