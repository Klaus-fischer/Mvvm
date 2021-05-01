// <copyright file="IEventCommand{T}.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
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
        event EventHandler<CanExecuteEventArgs<T>>? OnCanExecute;

        /// <summary>
        /// EventHandler to Execute the command.
        /// </summary>
        event EventHandler<T>? OnExecute;
    }
}
