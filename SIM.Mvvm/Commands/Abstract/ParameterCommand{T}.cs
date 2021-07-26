// <copyright file="ParameterCommand{T}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

#nullable disable

namespace SIM.Mvvm
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    /// <summary>
    /// Base command class that provides type conversation.
    /// Signature of <see cref="IParameterCommand{T}"/> implemented.
    /// </summary>
    /// <typeparam name="T">Type of the expected command parameter.</typeparam>
    public abstract class ParameterCommand<T> : IParameterCommand<T>, ICommandInvokeCanExecuteChangedEvent
    {
        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="default(T)"/> is a valid value.
        /// </summary>
        protected bool AllowDefault { get; set; } = true;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ICommand.CanExecute(object parameter)
        {
            if (parameter is T param)
            {
                return this.CanExecute(param);
            }

            if (!this.AllowDefault)
            {
                return false;
            }

            return this.CanExecute(default);
        }

        /// <inheritdoc/>
        void ICommand.Execute(object parameter)
        {
            if (parameter is T param)
            {
                this.Execute(param);
                return;
            }

            if (!this.AllowDefault)
            {
                return;
            }

            this.Execute(default);
        }

        /// <inheritdoc/>
        void ICommandInvokeCanExecuteChangedEvent.InvokeCanExecuteChanged(object sender, EventArgs e)
        {
            this.CanExecuteChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Execute to run this command.
        /// </summary>
        /// <param name="parameter">The parameter to execute.</param>
        public void Execute(T parameter)
        {
            if (parameter is not T param)
            {
                if (!this.AllowDefault)
                {
                    return;
                }

                param = default;
            }

            if (this.CanExecute(param))
            {
                this.OnExecute(param);
            }
        }

        /// <summary>
        /// Elevates if the command can be executed (<see cref="ICommand.CanExecute(object?)"/>).
        /// </summary>
        /// <param name="parameter">Converted parameter.</param>
        /// <returns>True if command is able to run.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool CanExecute(T parameter)
            => true;

        /// <summary>
        /// Runs the command (<see cref="ICommand.Execute(object?)"/>).
        /// </summary>
        /// <param name="parameter">Converted parameter.</param>
        protected abstract void OnExecute(T parameter);
    }
}
