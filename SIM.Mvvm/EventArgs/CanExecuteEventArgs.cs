// <copyright file="CanExecuteEventArgs.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    /// <summary>
    /// Event arguments for <see cref="IEventCommand"/> CanExecute handler.
    /// </summary>
    public class CanExecuteEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed of not.
        /// Default is true.
        /// </summary>
        public bool CanExecute { get; set; } = true;
    }
}
