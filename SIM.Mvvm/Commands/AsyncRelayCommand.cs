// <copyright file="AsyncRelayCommand.cs" company="SIM Automation">
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
    public class AsyncRelayCommand : Command
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

            if (this.context is IViewModel viewModel)
            {
                viewModel[nameof(IAsyncExecutionContext.IsBusy)].RegisterCommand(this);
            }
        }

        /// <summary>
        /// Gets or sets a handler for exceptions that occurs in <see cref="ExecuteAsync"/> method.
        /// </summary>
        public IExceptionHandler? ExceptionHandler { get; set; }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// If the command does not require data to be passed, this object can be set to null.
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ExecuteAsync()
        {
            if (!this.CanExecute())
            {
                return;
            }

            await this.RunAsync();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override sealed bool CanExecute()
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
        protected override sealed void OnExecute()
            => _ = this.RunAsync();

        private async Task RunAsync()
        {
            try
            {
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
