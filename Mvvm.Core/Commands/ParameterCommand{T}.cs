// <copyright file="ParameterCommand{T}.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    /// <summary>
    /// Base command class that provides type conversation.
    /// Signature of <see cref="IParameterCommand{T}"/> implemented.
    /// </summary>
    /// <typeparam name="T">Type of the expected command parameter.</typeparam>
    public abstract class ParameterCommand<T> : BaseCommand, IParameterCommand<T>, ICommandInvokeCanExecuteChangedEvent
    {
        /// <inheritdoc/>
        public sealed override bool CanExecute(object? parameter)
            => parameter is T param && this.CanExecute(param);

        /// <inheritdoc/>
        public sealed override void Execute(object? parameter)
        {
            if (parameter is T param && this.CanExecute(param))
            {
                this.Execute(param);
            }
        }

        /// <inheritdoc/>
        bool IParameterCommand<T>.CanExecute(T parameter) => this.CanExecute(parameter);

        /// <inheritdoc/>
        void IParameterCommand<T>.Execute(T parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.Execute(parameter);
            }
        }

        /// <summary>
        /// Elevates if the command can be executed (<see cref="CanExecute(object?)"/>).
        /// </summary>
        /// <param name="parameter">Converted parameter.</param>
        /// <returns>True if command is able to run.</returns>
        protected abstract bool CanExecute(T parameter);

        /// <summary>
        /// Runs the command (<see cref="Execute(object?)"/>).
        /// </summary>
        /// <param name="parameter">Converted parameter.</param>
        protected abstract void Execute(T parameter);
    }
}
