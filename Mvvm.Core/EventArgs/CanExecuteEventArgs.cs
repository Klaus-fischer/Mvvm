// <copyright file="CanExecuteEventArgs.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
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
