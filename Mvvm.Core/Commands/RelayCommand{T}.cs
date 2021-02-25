// <copyright file="RelayCommand{T}.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;

    /// <summary>
    /// Relay command to implement command behavior.
    /// </summary>
    /// <typeparam name="T">Type of expected parameter.</typeparam>
    public sealed class RelayCommand<T> : ParameterCommand<T>
    {
        private readonly Func<T?, bool>? canExecuteHandler;
        private readonly Action<T?> executeHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
        /// </summary>
        /// <param name="onExecute">Handler for execute command.</param>
        /// <param name="onCanExecute">Handler to validate whether execution is possible.</param>
        public RelayCommand(Action<T?> onExecute, Func<T?, bool>? onCanExecute = null)
        {
            this.canExecuteHandler = onCanExecute;
            this.executeHandler = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
        }

        /// <inheritdoc/>
        protected override sealed bool CanExecute(T parameter)
        {
            return this.canExecuteHandler?.Invoke(parameter) ?? true;
        }

        /// <inheritdoc/>
        protected override sealed void Execute(T parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.executeHandler(parameter);
            }
        }
    }
}
