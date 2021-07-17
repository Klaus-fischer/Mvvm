﻿// <copyright file="BaseCommand.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    /// <summary>
    /// Base command class that implements <see cref="ICommandInvokeCanExecuteChangedEvent"/>.
    /// </summary>
    public abstract class BaseCommand : ICommandInvokeCanExecuteChangedEvent
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        bool ICommand.CanExecute(object parameter)
            => this.CanExecute();

        /// <inheritdoc/>
        void ICommand.Execute(object parameter)
             => this.Execute();

        /// <inheritdoc/>
        void ICommandInvokeCanExecuteChangedEvent.InvokeCanExecuteChanged(object? sender, EventArgs e)
        {
            this.CanExecuteChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Execute to run this command.
        /// </summary>
        public void Execute()
        {
            if (this.CanExecute())
            {
                this.OnExecute();
            }
        }

        /// <summary>
        /// Call to check if the command can be executed.
        /// </summary>
        /// <returns>True if context is valid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool CanExecute()
            => true;

        /// <summary>
        /// The execution context.
        /// The <see cref="CanExecute"/> call was already checked.
        /// </summary>
        protected abstract void OnExecute();
    }
}
