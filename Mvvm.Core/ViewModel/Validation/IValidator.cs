// <copyright file="IValidator.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
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