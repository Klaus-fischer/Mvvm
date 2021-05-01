// <copyright file="AsyncRelayCommand{T}.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core.Async
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Relay command to implement asynchron command behavior.
    /// </summary>
    /// <typeparam name="T">The expected type of the command parameter.</typeparam>
    public sealed class AsyncRelayCommand<T> : ParameterCommand<T>
    {
        private readonly IAsyncExecutionContext context;
        private readonly Func<T?, bool>? canExecuteHandler;
        private readonly AsyncCommandExecutionHandler<T?> executeHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
        /// </summary>
        /// <param name="context">Synchronization context to run into.</param>
        /// <param name="onExecute">Asynchron handler for execute command.</param>
        /// <param name="onCanExecute">Handler to validate whether execution is possible.</param>
        public AsyncRelayCommand(
            IAsyncExecutionContext context,
            AsyncCommandExecutionHandler<T?> onExecute,
            Func<T?, bool>? onCanExecute = null)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.executeHandler = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
            this.canExecuteHandler = onCanExecute;

            _ = this.RegisterPropertyDependency(this.context, nameof(IAsyncExecutionContext.IsBusy));
        }

        /// <summary>
        /// Gets or sets a handler for exceptions that occurs in <see cref="onExecute"/> method.
        /// </summary>
        public IExceptionHandler? ExceptionHandler { get; set; }

        /// <inheritdoc/>
        public override bool CanExecute(T parameter)
        {
            if (this.context.IsBusy)
            {
                return false;
            }

            // tries to execute the can execute handler, if defined. Otherwise return true as default.
            if (this.canExecuteHandler != null)
            {
                return this.canExecuteHandler(parameter);
            }
            else
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public override void Execute(T? parameter)
        {
            _ = this.ExecuteAsync(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.
        /// If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>Asynchron task to wait for.</returns>
        public async Task ExecuteAsync(T? parameter)
        {
            try
            {
                if (!this.CanExecute(parameter))
                {
                    return;
                }

                this.context.PrepareExecution(out var token);

                await this.executeHandler.Invoke(parameter, token);
            }
            catch (Exception ex)
            {
                if (this.ExceptionHandler?.HandleException(ex) ?? true)
                {
                    throw;
                }
            }
            finally
            {
                this.context.FinalizeExecution();
            }
        }
    }
}
