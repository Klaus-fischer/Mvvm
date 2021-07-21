// <copyright file="IValidator.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace Sim.Mvvm.Validation
{
    using System.ComponentModel;

    /// <summary>
    /// Extends <see cref="INotifyDataErrorInfo"/> interface by <see cref="Validate"/> Method.
    /// </summary>
    public interface IValidator : INotifyDataErrorInfo
    {
        /// <summary>
        /// This method will validate all public properties.
        /// </summary>
        /// <returns>False if any error.</returns>
        bool Validate();
    }
}