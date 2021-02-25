// <copyright file="BaseCommand.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Base command class that implements <see cref="ICommandInvokeCanExecuteChangedEvent"/>.
    /// </summary>
    public abstract class BaseCommand : ICommand, ICommandInvokeCanExecuteChangedEvent
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        public abstract bool CanExecute(object parameter);

        /// <inheritdoc/>
        public abstract void Execute(object parameter);

        /// <inheritdoc/>
        void ICommandInvokeCanExecuteChangedEvent.RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
