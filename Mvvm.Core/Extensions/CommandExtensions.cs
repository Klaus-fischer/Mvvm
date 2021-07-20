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
        private static Collection<ViewModelCommandListener> listeners
            = new Collection<ViewModelCommandListener>();

        /// <summary>
        /// Creates an event listener to monitor depend properties.
        /// Raises <see cref="ICommand.CanExecuteChanged"/> event, if property was changed.
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
        public static T RegisterPropertyDependency<T>(
            this T command,
            INotifyPropertyChanged viewModel,
            params string[] dependencies)
            where T : ICommandInvokeCanExecuteChangedEvent
        {
            if (dependencies is null || !dependencies.Any())
            {
                throw new ArgumentException("dependencies must contain at least one PropertyName");
            }

            var listener = new ViewModelCommandListener(
                viewModel ?? throw new ArgumentNullException(nameof(viewModel)),
                command ?? throw new ArgumentNullException(nameof(command)),
                dependencies);

            listeners.Add(listener);

            return command;
        }

        /// <summary>
        /// Unregisters the command listener.
        /// </summary>
        /// <param name="command">The command to release the monitor.</param>
        /// <param name="viewModel">The view model the command belongs tó.</param>
        public static void UnregisterPropertyDependency(
            this ICommandInvokeCanExecuteChangedEvent command,
            INotifyPropertyChanged viewModel)
        {
            if (listeners.FirstOrDefault(o => o.BelongsTo(viewModel, command)) is ViewModelCommandListener listener)
            {
                listener.Dispose();
                listeners.Remove(listener);
            }
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
    }
}
