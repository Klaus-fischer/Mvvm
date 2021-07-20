// <copyright file="IAsyncExecutionContext.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Input;

    /// <summary>
    /// Declaration of an asynchron execution context.
    /// </summary>
    public interface IAsyncExecutionContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating whether the execution context is busy.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Gets the command to cancel the current running command.
        /// </summary>
        ICommand Cancel { get; }

        /// <summary>
        /// Gets the current active cancellation token source.
        /// </summary>
        CancellationTokenSource? CancellationTokenSource { get;  }

        /// <summary>
        /// Prepares an execution context.
        /// <see cref="IsBusy"/> will be set after this call.
        /// </summary>
        /// <param name="token">The cancellation token to cancel the asynchron command.</param>
        void PrepareExecution(out CancellationToken token);

        /// <summary>
        /// This call will finalize the context execution.
        /// <see cref="IsBusy"/> must be cleared after this call.
        /// </summary>
        void FinalizeExecution();
    }
}