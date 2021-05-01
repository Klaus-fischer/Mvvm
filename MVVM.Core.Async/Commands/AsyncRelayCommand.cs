// <copyright file="AsyncRelayCommand.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core.Async
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Relay command to implement asynchron command behavior.
    /// </summary>
    public class AsyncRelayCommand : BaseCommand
    {
        private readonly IAsyncExecutionContext context;
        private readonly Func<bool>? canExecuteHandler;
        private readonly AsyncCommandExecutionHandler executeHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="context">Synchronization context to run into.</param>
        /// <param name="onExecute">Asynchron handler for execute command.</param>
        /// <param name="onCanExecute">Handler to validate whether execution is possible.</param>
        public AsyncRelayCommand(
            IAsyncExecutionContext context,
            AsyncCommandExecutionHandler onExecute,
            Func<bool>? onCanExecute = null)
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
        public override sealed bool CanExecute(object? parameter)
        {
            if (this.context.IsBusy)
            {
                return false;
            }

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
        public override void Execute(object? parameter)
        {
            _ = this.ExecuteAsync(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.
        /// If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>Asynchron task to wait for.</returns>
        public async Task ExecuteAsync(object? parameter)
        {
            try
            {
                if (!this.CanExecute(parameter))
                {
                    return;
                }

                this.context.PrepareExecution(out var token);

                await this.executeHandler.Invoke(token);
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
