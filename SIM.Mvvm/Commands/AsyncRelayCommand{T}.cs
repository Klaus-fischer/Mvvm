// <copyright file="AsyncRelayCommand{T}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Relay command to implement asynchron command behavior.
    /// </summary>
    /// <typeparam name="T">The expected type of the command parameter.</typeparam>
    public sealed class AsyncRelayCommand<T> : ParameterCommand<T>
    {
        private readonly IAsyncExecutionContext context;
        private readonly Func<T?, bool>? canExecuteHandler;
        private readonly AsyncCommandExecutionHandler<T> executeHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
        /// </summary>
        /// <param name="context">Synchronization context to run into.</param>
        /// <param name="onExecute">Asynchron handler for execute command.</param>
        /// <param name="onCanExecute">Handler to validate whether execution is possible.</param>
        public AsyncRelayCommand(
            IAsyncExecutionContext context,
            AsyncCommandExecutionHandler<T> onExecute,
            Func<T?, bool>? onCanExecute = null)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.executeHandler = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
            this.canExecuteHandler = onCanExecute;

            if (this.context is IViewModel viewModel)
            {
                viewModel[nameof(IAsyncExecutionContext.IsBusy)].RegisterCommand(this);
            }
        }

        /// <summary>
        /// Gets or sets a handler for exceptions that occurs in <see cref="onExecute"/> method.
        /// </summary>
        public IExceptionHandler? ExceptionHandler { get; set; }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.
        /// If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(T parameter)
        {
            if (!this.CanExecute(parameter))
            {
                return;
            }

            await this.RunAsync(parameter);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override sealed bool CanExecute(T parameter)
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
        protected override sealed void OnExecute(T parameter) 
            => _ = this.RunAsync(parameter);

        private async Task RunAsync(T parameter)
        {
            try
            {
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
