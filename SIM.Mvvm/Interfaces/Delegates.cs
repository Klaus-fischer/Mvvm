// <copyright file="Delegates.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an execution handler of a <see cref="RelayCommand{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the expected parameter.</typeparam>
    /// <param name="parameter">The parameter of the command.</param>
    public delegate void CommandExecuteHandler<T>(T? parameter);

    /// <summary>
    /// Defines an can execute handler of a <see cref="RelayCommand{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the expected parameter.</typeparam>
    /// <param name="parameter">The parameter of the command.</param>
    /// <returns>True if the command can be executed.</returns>
    public delegate bool CommandCanExecuteHandler<T>(T? parameter);

    /// <summary>
    /// Defines an execution handler of an <see cref="AsyncRelayCommand"/>.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The asynchronous task.</returns>
    public delegate Task AsyncCommandExecutionHandler(CancellationToken token);

    /// <summary>
    /// Defines an execution handler of an <see cref="AsyncRelayCommand{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the expected parameter.</typeparam>
    /// <param name="parameter">The parameter of the command.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The asynchronous task.</returns>
    public delegate Task AsyncCommandExecutionHandler<T>(T? parameter, CancellationToken token);
}
