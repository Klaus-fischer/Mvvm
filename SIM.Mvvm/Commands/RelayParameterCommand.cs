// <copyright file="RelayParameterCommand.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    /// <summary>
    /// Decorator pattern to inject a command parameter to a parameterless command.
    /// </summary>
    public class RelayParameterCommand : Command, ICommand
    {
        private readonly Func<object> getParameter;
        private readonly ICommand targetCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayParameterCommand"/> class.
        /// </summary>
        /// <param name="targetCommand">The Command to relay to.</param>
        /// <param name="parameterCallback">Callback to parameter to pass into command parameter.</param>
        public RelayParameterCommand(ICommand targetCommand, Func<object> parameterCallback)
        {
            this.targetCommand = targetCommand ?? throw new ArgumentNullException(nameof(targetCommand));
            this.getParameter = parameterCallback ?? throw new ArgumentNullException(nameof(parameterCallback));

            this.targetCommand.CanExecuteChanged += (s, a) => this.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayParameterCommand"/> class.
        /// </summary>
        /// <param name="targetCommand">The Command to relay to.</param>
        /// <param name="parameter">The parameter to pass into command calls.</param>
        public RelayParameterCommand(ICommand targetCommand, object parameter)
            : this(targetCommand, () => parameter)
        {
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override sealed bool CanExecute()
            => this.targetCommand.CanExecute(this.getParameter());

        /// <inheritdoc/>
        protected override sealed void OnExecute()
            => this.targetCommand.Execute(this.getParameter());
    }
}
