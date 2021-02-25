// <copyright file="ICommandInvokeCanExecuteChangedEvent.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System.Windows.Input;

    /// <summary>
    /// Interface to raise <see cref="ICommand.CanExecuteChanged"/> event.
    /// </summary>
    public interface ICommandInvokeCanExecuteChangedEvent
    {
        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
