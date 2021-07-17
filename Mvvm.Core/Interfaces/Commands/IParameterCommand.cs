// <copyright file="IParameterCommand.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
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
