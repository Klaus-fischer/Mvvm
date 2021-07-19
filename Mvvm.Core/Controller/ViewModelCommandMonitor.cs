// <copyright file="ViewModelCommandMonitor.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System.ComponentModel;

    /// <summary>
    /// Observer class to monitor a command of an view model.
    /// Will register <see cref="CommandExtensions.RegisterPropertyDependency{T}(T, INotifyPropertyChanged, string[])"/>
    /// on any change of the command.
    /// </summary>
    internal class ViewModelCommandMonitor
    {
        private readonly INotifyPropertyChanged target;
        private readonly string commandName;
        private readonly string[] dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCommandMonitor"/> class.
        /// </summary>
        /// <param name="target">Target view model to observe.</param>
        /// <param name="commandName">Name of the command to monitor.</param>
        /// <param name="dependencies">Collection of dependencies to monitor.</param>
        public ViewModelCommandMonitor(
            IViewModel target,
            string commandName,
            params string[] dependencies)
        {
            this.target = target;
            this.commandName = commandName;

            this.dependencies = dependencies;
            target.AdvancedPropertyChanged += this.RaiseCommandCanExecuteChangedOnTargetPropertyChanged;
        }

        private void RaiseCommandCanExecuteChangedOnTargetPropertyChanged(object sender, AdvancedPropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.commandName)
            {
                if (e.Before is ICommandInvokeCanExecuteChangedEvent oldCmd)
                {
                   oldCmd.UnregisterPropertyDependency(this.target);
                }

                if (e.After is ICommandInvokeCanExecuteChangedEvent newCmd)
                {
                    newCmd.RegisterPropertyDependency(this.target, this.dependencies);
                }
            }
        }
    }
}
