// <copyright file="IEventCommand.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Declaration of an non parameter event command.
    /// </summary>
    public interface IEventCommand : ICommand
    {
        /// <summary>
        /// EventHandler to determine can execute changed state.
        /// </summary>
        event EventHandler<CanExecuteEventArgs>? OnCanExecuted;

        /// <summary>
        /// EventHandler to Execute the command.
        /// </summary>
        event EventHandler? OnExecuted;
    }
}
