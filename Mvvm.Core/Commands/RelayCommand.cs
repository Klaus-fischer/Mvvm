// <copyright file="RelayCommand.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Relay command to implement command behavior.
    /// </summary>
    public sealed class RelayCommand : BaseCommand, ICommand
    {
        private readonly Func<bool>? canExecuteHandler;
        private readonly Action executeHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="onExecute">Handler for execute command.</param>
        /// <param name="onCanExecute">Handler to validate whether execution is possible.</param>
        public RelayCommand(
            Action onExecute,
            Func<bool>? onCanExecute = null)
        {
            this.canExecuteHandler = onCanExecute;
            this.executeHandler = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
        }

        /// <inheritdoc/>
        public override sealed bool CanExecute(object? parameter)
        {
            // tries to execute the can execute handler, if defined. Otherwise return true as default.
            if (this.canExecuteHandler != null)
            {
                return this.canExecuteHandler();
            }
            else
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public override sealed void Execute(object? parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.executeHandler();
            }
        }
    }
}
