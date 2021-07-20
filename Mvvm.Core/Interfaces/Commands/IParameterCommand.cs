// <copyright file="IParameterCommand.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System.Windows.Input;

    /// <summary>
    /// Definition of an command with a required parameter.
    /// </summary>
    /// <typeparam name="T">Type of the parameter that is required.</typeparam>
    public interface IParameterCommand<T> : ICommand
    {
    }
}
