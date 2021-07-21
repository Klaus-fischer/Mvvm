// <copyright file="ViewModelCommandListener.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Observer class to monitor a properties of a view model and
    /// fire <see cref="ICommand.CanExecuteChanged"/> if depend property changed.
    /// </summary>
    internal class ViewModelCommandListener : IDisposable
    {
        private readonly INotifyPropertyChanged target;
        private readonly ICommandInvokeCanExecuteChangedEvent command;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCommandListener"/> class.
        /// </summary>
        /// <param name="target">Target view model to observe.</param>
        /// <param name="command">Command to raise <see cref="ICommand.CanExecuteChanged"/>.</param>
        /// <param name="dependencies">Collection of dependencies to monitor.</param>
        public ViewModelCommandListener(
            IViewModel target,
            ICommandInvokeCanExecuteChangedEvent command,
            params string[] dependencies)
        {
            this.target = target;
            this.command = command;

            target[dependencies].RegisterCallback(this.RaiseCommandCanExecuteChangedOnTargetPropertyChanged);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.target.PropertyChanged -= this.RaiseCommandCanExecuteChangedOnTargetPropertyChanged;
        }

        /// <summary>
        /// Checks if current listener belongs to the view model and the command.
        /// </summary>
        /// <param name="viewModel">the view model to check.</param>
        /// <param name="command">The command to check.</param>
        /// <returns>True if view model and command matches.</returns>
        internal bool BelongsTo(INotifyPropertyChanged viewModel, ICommandInvokeCanExecuteChangedEvent command)
            => this.target == viewModel && command == this.command;

        private void RaiseCommandCanExecuteChangedOnTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.command.InvokeCanExecuteChanged(this.command, new EventArgs());
        }
    }
}
