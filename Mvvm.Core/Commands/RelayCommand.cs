// <copyright file="RelayCommand.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    /// <summary>
    /// Relay command to implement command behavior.
    /// </summary>
    public class RelayCommand : Command, ICommand
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override sealed bool CanExecute()
            => this.canExecuteHandler?.Invoke() ?? true;

        /// <inheritdoc/>
        protected override sealed void OnExecute()
            => this.executeHandler();
    }
}
