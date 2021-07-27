// <copyright file="CommandExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Extension class to extend <see cref="ICommand"/> (<see cref="INotifyCommand"/>).
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Tries to invoke the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="command">The command that has changed.</param>
        public static void TryInvokeCanExecuteChanged(this ICommand command)
        {
            if (command is INotifyCommand cmd)
            {
                cmd.NotifyCanExecuteChanged();
                return;
            }

            throw new InvalidOperationException("The event could not be invoked.");
        }
    }
}
