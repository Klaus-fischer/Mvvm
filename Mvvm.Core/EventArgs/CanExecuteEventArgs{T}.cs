// <copyright file="CanExecuteEventArgs{T}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    /// <summary>
    /// Event arguments for <see cref="IEventCommand"/> CanExecute handler.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public class CanExecuteEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanExecuteEventArgs{T}"/> class.
        /// </summary>
        /// <param name="parameter">The parameter of the event arguments.</param>
        public CanExecuteEventArgs(T parameter)
        {
            this.Parameter = parameter;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// Default is true.
        /// </summary>
        public bool CanExecute { get; set; } = true;

        /// <summary>
        /// Gets the command parameter.
        /// </summary>
        public T Parameter { get; }
    }
}
