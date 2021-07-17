// <copyright file="EventCommand{T}.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Implementation of an <see cref="IEventCommand"/>.
    /// </summary>
    /// <typeparam name="T">Expected parameter type.</typeparam>
    public class EventCommand<T> : ParameterCommand<T>, IEventCommand<T>
    {
        /// <inheritdoc/>
        public event EventHandler<CanExecuteEventArgs<T>>? OnCanExecuted;

        /// <inheritdoc/>
        public event EventHandler<T>? OnExecuted;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override sealed bool CanExecute(T parameter)
        {
            if (this.OnExecuted is null)
            {
                return false;
            }

            var args = new CanExecuteEventArgs<T>(parameter) { CanExecute = true };
            this.OnCanExecuted?.Invoke(this, args);

            return args.CanExecute;
        }

        /// <inheritdoc/>
        protected override sealed void OnExecute(T parameter)
            => this.OnExecuted?.Invoke(this, parameter);
    }
}
