// <copyright file="CommandExtensions.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Extension class to extend <see cref="ICommand"/> (<see cref="ICommandInvokeCanExecuteChangedEvent"/>).
    /// </summary>
    public static class CommandExtensions
    {
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

            _ = new ViewModelCommandListener(
                viewModel ?? throw new ArgumentNullException(nameof(viewModel)),
                command ?? throw new ArgumentNullException(nameof(command)),
                dependencies);

            return command;
        }
    }
}
