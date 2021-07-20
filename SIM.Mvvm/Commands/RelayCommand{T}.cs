// <copyright file="RelayCommand{T}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Relay command to implement command behavior.
    /// </summary>
    /// <typeparam name="T">Type of expected parameter.</typeparam>
    public class RelayCommand<T> : ParameterCommand<T>
    {
        private readonly CommandCanExecuteHandler<T?>? canExecuteHandler;
        private readonly CommandExecuteHandler<T?> executeHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
        /// </summary>
        /// <param name="onExecute">Handler for execute command.</param>
        /// <param name="onCanExecute">Handler to validate whether execution is possible.</param>
        public RelayCommand(
            CommandExecuteHandler<T?> onExecute,
            CommandCanExecuteHandler<T?>? onCanExecute = null)
        {
            this.canExecuteHandler = onCanExecute;
            this.executeHandler = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override sealed bool CanExecute(T parameter) 
            => this.canExecuteHandler?.Invoke(parameter) ?? true;

        /// <inheritdoc/>
        protected override sealed void OnExecute(T parameter)
            => this.executeHandler(parameter);
    }
}
