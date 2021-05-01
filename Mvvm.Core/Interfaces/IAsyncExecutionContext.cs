// <copyright file="IAsyncExecutionContext.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System.ComponentModel;
    using System.Threading;

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