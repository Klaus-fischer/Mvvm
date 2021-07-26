// <copyright file="CommandExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Extension class to extend <see cref="ICommand"/> (<see cref="ICommandInvokeCanExecuteChangedEvent"/>).
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Creates an <see cref="IPropertyMonitor"/> to listen to monitor depend properties.
        /// Raises <see cref="ICommand.CanExecuteChanged"/> event, if property was changed.
        /// Use <see cref="RegisterPropertyDependency{T}(T, IViewModel, string[])"/> instead this call.
        /// </summary>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <param name="command">The Command to notify.</param>
        /// <param name="viewModel">The view model that contains the properties.</param>
        /// <param name="dependencies">Collection of property names the command depends on.</param>
        /// <returns>The command itself.</returns>
        /// <example>
        /// public class MyViewModel : IViewModel
        /// {
        ///     public int Property { get; set; }
        ///     public ICommand MyCommand { get; }
        ///
        ///     MyViewModel()
        ///     {
        ///         MyCommand = new RelayCommand(doSomething, () => Property == 0)
        ///             .RegisterPropertyDependency(this, nameof(Property));
        ///     }
        /// }
        /// ...
        /// </example>
        [Obsolete("Use IViewModel instead.")]
        public static T RegisterPropertyDependency<T>(
            this T command,
            INotifyPropertyChanged viewModel,
            params string[] dependencies)
            where T : ICommandInvokeCanExecuteChangedEvent
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (dependencies is null || !dependencies.Any())
            {
                throw new ArgumentException("dependencies must contain at least one PropertyName");
            }

            foreach (var prop in dependencies)
            {
                var monitor = PropertyMonitorFactory.Create(viewModel, prop);
                monitor.RegisterCommand(command);
            }

            return command;
        }

        /// <summary>
        /// Registers Command to <see cref="IPropertyMonitor"/> of the view model.
        /// Raises <see cref="ICommand.CanExecuteChanged"/> event, if property was changed.
        /// </summary>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <param name="command">The Command to notify.</param>
        /// <param name="viewModel">The view model that contains the properties.</param>
        /// <param name="dependencies">Collection of property names the command depends on.</param>
        /// <returns>The command itself.</returns>
        public static T RegisterPropertyDependency<T>(
            this T command,
            IViewModel viewModel,
            params string[] dependencies)
            where T : ICommandInvokeCanExecuteChangedEvent
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (dependencies is null || !dependencies.Any())
            {
                throw new ArgumentException("dependencies must contain at least one PropertyName");
            }

            viewModel[dependencies].RegisterCommand(command);

            return command;
        }

        /// <summary>
        /// Unregisters the command listener.
        /// </summary>
        /// <param name="command">The command to release the monitor.</param>
        /// <param name="viewModel">The view model the command belongs to.</param>
        public static void UnregisterPropertyDependency(
            this ICommandInvokeCanExecuteChangedEvent command,
            IViewModel viewModel)
        {
            viewModel[ViewModel.AllPropertyMontitorsToUnregister].UnregisterCommand(command);
        }

        /// <summary>
        /// Tries to invoke the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="command">The command that has changed.</param>
        public static void TryInvokeCanExecuteChanged(this ICommand command)
        {
            if (command is ICommandInvokeCanExecuteChangedEvent cmd)
            {
                cmd.InvokeCanExecuteChanged(cmd, new EventArgs());
                return;
            }

            throw new InvalidOperationException("The event could not be invoked.");
        }

        /// <summary>
        /// Tries to invoke the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="command">The command that has changed.</param>
        public static void InvokeCanExecuteChanged(this ICommandInvokeCanExecuteChangedEvent command)
        {
            command.InvokeCanExecuteChanged(command, new EventArgs());
        }
    }
}
