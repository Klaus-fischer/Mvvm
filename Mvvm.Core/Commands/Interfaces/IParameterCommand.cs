// <copyright file="IParameterCommand.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    /// <summary>
    /// Definition of an command with a required parameter.
    /// </summary>
    /// <typeparam name="T">Type of the parameter that is required.</typeparam>
    public interface IParameterCommand<T>
    {
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"> Data used by the command.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        bool CanExecute(T parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        void Execute(T parameter);
    }
}
