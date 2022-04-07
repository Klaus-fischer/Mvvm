// <copyright file="CommandNotifier.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Holds command structure to notify on dependency changed.
    /// </summary>
    internal class CommandNotifier : ICommandNotifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotifier"/> class.
        /// </summary>
        /// <param name="viewModel">the view model the command belongs to.</param>
        /// <param name="commandName">The name of the command property in the view model.</param>
        public CommandNotifier(INotifyPropertyChanged viewModel, string commandName)
        {
            this.Target = viewModel;
            this.CommandName = commandName;
            viewModel.PropertyChanged += this.UpdateCommand;

            // to set the current command.
            this.UpdateCommand(this, new PropertyChangedEventArgs(this.CommandName));
        }

        /// <inheritdoc/>
        public string CommandName { get; }

        /// <inheritdoc/>
        public INotifyPropertyChanged? Target { get; }

        private INotifyCommand? Command { get; set; }

        /// <inheritdoc/>
        public void NotifyCommandChanged()
        {
            this.Command?.NotifyCanExecuteChanged();
        }

        private void UpdateCommand(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.CommandName)
            {
                if (this.Target is INotifyPropertyChanged viewModel)
                {
                    var getter = Expression.Lambda(
                        Expression.Property(
                            Expression.Constant(viewModel),
                            this.CommandName)).Compile();

                    this.Command = getter.DynamicInvoke() as INotifyCommand;
                }
            }
        }
    }
}
