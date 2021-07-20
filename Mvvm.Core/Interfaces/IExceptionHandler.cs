// <copyright file="IExceptionHandler.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;

    /// <summary>
    /// Declaration of an execution handler.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Handles an Exception.
        /// </summary>
        /// <param name="ex">Exception to handle.</param>
        /// <returns>If exception could not be handled and should be re thrown.</returns>
        bool HandleException(Exception ex);
    }
}
