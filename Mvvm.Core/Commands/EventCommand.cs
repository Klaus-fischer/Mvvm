// <copyright file="EventCommand.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Implementation of an <see cref="IEventCommand"/>.
    /// </summary>
    public class EventCommand : BaseCommand, IEventCommand
    {
        /// <inheritdoc/>
        public event EventHandler<CanExecuteEventArgs>? OnCanExecuted;

        /// <inheritdoc/>
        public event EventHandler? OnExecuted;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override sealed bool CanExecute()
        {
            if (this.OnExecuted is null)
            {
                return false;
            }

            var args = new CanExecuteEventArgs() { CanExecute = true };
            this.OnCanExecuted?.Invoke(this, args);

            return args.CanExecute;
        }

        /// <inheritdoc/>
        protected override sealed void OnExecute() 
            => this.OnExecuted?.Invoke(this, new EventArgs());
    }
}
