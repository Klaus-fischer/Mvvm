// <copyright file="AsyncExecutionContext.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Threading;
    using System.Windows.Input;
    using SIM.Mvvm.Expressions;

    /// <summary>
    /// Execution context for asynchron commands.
    /// </summary>
    public class AsyncExecutionContext : ViewModel, IAsyncExecutionContext
    {
        private bool isBusy;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncExecutionContext"/> class.
        /// </summary>
        public AsyncExecutionContext()
        {
            this.Cancel = new RelayCommand(this.OnCancel, this.CanCancel);

            this.Listen(() => this.IsBusy)
                .Notify(() => this.CancellationTokenSource, () => this.Cancel);
        }

        /// <inheritdoc/>
        public CancellationTokenSource? CancellationTokenSource { get; private set; }

        /// <inheritdoc/>
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

            this.CancellationTokenSource = new CancellationTokenSource();
            this.IsBusy = true;
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
