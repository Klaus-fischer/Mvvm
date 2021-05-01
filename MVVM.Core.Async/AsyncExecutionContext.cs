// <copyright file="AsyncExecutionContext.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core.Async
{
    using System;
    using System.Threading;
    using System.Windows.Input;

    /// <summary>
    /// Execution context for asynchron commands.
    /// </summary>
    public class AsyncExecutionContext : BaseViewModel, IAsyncExecutionContext
    {
        private bool isBusy;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncExecutionContext"/> class.
        /// </summary>
        public AsyncExecutionContext()
        {
            this.Cancel = new RelayCommand(this.OnCancel, this.CanCancel)
                .RegisterPropertyDependency(this, nameof(this.IsBusy));
        }

        /// <summary>
        /// Gets the current active cancellation token source.
        /// </summary>
        public CancellationTokenSource? CancellationTokenSource { get; private set; }

        /// <summary>
        /// Gets the command to cancel the current running command.
        /// </summary>
        public ICommand Cancel { get; }

        /// <inheritdoc/>
        public bool IsBusy
        {
            get => this.isBusy;
            private set => this.SetPropertyValue(ref this.isBusy, value);
        }

        /// <inheritdoc/>
        public void PrepareExecution(out CancellationToken token)
        {
            if (this.IsBusy)
            {
                throw new InvalidOperationException("Context is busy.");
            }

            this.IsBusy = true;
            this.CancellationTokenSource = new CancellationTokenSource();
            token = this.CancellationTokenSource.Token;
        }

        /// <inheritdoc/>
        public void FinalizeExecution()
        {
            this.IsBusy = false;
            this.CancellationTokenSource = null;
        }

        private bool CanCancel() => this.IsBusy;

        private void OnCancel() => this.CancellationTokenSource?.Cancel();
    }
}
