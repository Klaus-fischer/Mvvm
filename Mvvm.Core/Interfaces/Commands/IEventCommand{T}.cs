// <copyright file="IEventCommand{T}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;

    /// <summary>
    /// Declaration of an non parameter event command.
    /// </summary>
    /// <typeparam name="T">Expected parameter type.</typeparam>
    public interface IEventCommand<T> : IParameterCommand<T>
    {
        /// <summary>
        /// EventHandler to determine can execute changed state.
        /// </summary>
        event EventHandler<CanExecuteEventArgs<T>>? OnCanExecuted;

        /// <summary>
        /// EventHandler to Execute the command.
        /// </summary>
        event EventHandler<T>? OnExecuted;
    }
}
