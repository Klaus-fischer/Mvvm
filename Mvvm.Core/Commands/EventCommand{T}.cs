// <copyright file="EventCommand{T}.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;

    /// <summary>
    /// Implementation of an <see cref="IEventCommand"/>.
    /// </summary>
    /// <typeparam name="T">Expected parameter type.</typeparam>
    public sealed class EventCommand<T> : ParameterCommand<T>, IEventCommand<T>
    {
        /// <inheritdoc/>
        public event EventHandler<CanExecuteEventArgs<T>>? OnCanExecute;

        /// <inheritdoc/>
        public event EventHandler<T>? OnExecute;

        /// <inheritdoc/>
        protected override sealed bool CanExecute(T parameter)
        {
            if (this.OnExecute is null)
            {
                return false;
            }

            var args = new CanExecuteEventArgs<T>(parameter) { CanExecute = true };
            this.OnCanExecute?.Invoke(this, args);

            return args.CanExecute;
        }

        /// <inheritdoc/>
        protected override sealed void Execute(T parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.OnExecute?.Invoke(this, parameter);
            }
        }
    }
}
