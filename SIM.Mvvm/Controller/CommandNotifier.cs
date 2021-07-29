// <copyright file="CommandNotifier.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Holds command structure to notify on dependency changed.
    /// </summary>
    internal class CommandNotifier : ICommandNotifier
    {
        private readonly WeakReference<INotifyCommand?> commandReference;
        private readonly WeakReference<INotifyPropertyChanged> viewModelReference;
        private readonly int viewModelHash;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotifier"/> class.
        /// </summary>
        /// <param name="viewModel">the view model the command belongs to.</param>
        /// <param name="commandName">The name of the command property in the view model.</param>
        public CommandNotifier(INotifyPropertyChanged viewModel, string commandName)
        {
            this.commandReference = new WeakReference<INotifyCommand?>(null);
            this.viewModelReference = new WeakReference<INotifyPropertyChanged>(viewModel);
            this.CommandName = commandName;
            this.viewModelHash = viewModel.GetHashCode();
            viewModel.PropertyChanged += this.UpdateCommand;

            // to set the current command.
            this.UpdateCommand(this, new PropertyChangedEventArgs(this.CommandName));
        }

        /// <inheritdoc/>
        public string CommandName { get; }

        /// <inheritdoc/>
        public INotifyPropertyChanged? Target
        {
            get
            {
                if (this.viewModelReference.TryGetTarget(out var value))
                {
                    return value;
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public void NotifyCommandChanged()
        {
            if (this.commandReference.TryGetTarget(out var command))
            {
                command.NotifyCanExecuteChanged();
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => (this.viewModelHash * 269) + this.CommandName.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is CommandNotifier c)
            {
                if (c.CommandName != this.CommandName ||
                    c.viewModelHash != this.viewModelHash)
                {
                    return false;
                }

                if (c.viewModelReference.TryGetTarget(out var cViewModel) &&
                    this.viewModelReference.TryGetTarget(out var thisViewModel) &&
                    ReferenceEquals(cViewModel, thisViewModel))
                {
                    return true;
                }

                return false;
            }

            return base.Equals(obj);
        }

        private void UpdateCommand(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.CommandName)
            {
                if (this.viewModelReference.TryGetTarget(out var viewModel))
                {
                    var getter = Expression.Lambda(
                        Expression.Property(
                            Expression.Constant(viewModel),
                            this.CommandName)).Compile();

                    var command = getter.DynamicInvoke() as INotifyCommand;

                    this.commandReference.SetTarget(command);
                }
            }
        }
    }
}
