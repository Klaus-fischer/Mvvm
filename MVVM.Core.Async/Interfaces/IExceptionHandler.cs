// <copyright file="IExceptionHandler.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core.Async
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
