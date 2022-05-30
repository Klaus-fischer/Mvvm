// <copyright file="Command.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    /// <summary>
    /// Base command class that implements <see cref="INotifyCommand"/>.
    /// </summary>
    public abstract class Command : INotifyCommand
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
        public void NotifyCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
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
