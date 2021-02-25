// <copyright file="EventCommand.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;

    /// <summary>
    /// Implementation of an <see cref="IEventCommand"/>.
    /// </summary>
    public sealed class EventCommand : BaseCommand, IEventCommand
    {
        /// <inheritdoc/>
        public event EventHandler<CanExecuteEventArgs>? OnCanExecute;

        /// <inheritdoc/>
        public event EventHandler? OnExecute;

        /// <inheritdoc/>
        public override sealed bool CanExecute(object parameter)
        {
            if (this.OnExecute is null)
            {
                return false;
            }

            var args = new CanExecuteEventArgs() { CanExecute = true };
            this.OnCanExecute?.Invoke(this, args);

            return args.CanExecute;
        }

        /// <inheritdoc/>
        public override sealed void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.OnExecute?.Invoke(this, new EventArgs());
            }
        }
    }
}
