// <copyright file="AdvancedPropertyChangedEventArgs.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System.ComponentModel;

    /// <summary>
    /// Extends <see cref="PropertyChangedEventArgs"/> by <see cref="Before"/> and <see cref="After"/> properties.
    /// </summary>
    public class AdvancedPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property that was changed.</param>
        /// <param name="before">The old value.</param>
        /// <param name="after">The new value.</param>
        public AdvancedPropertyChangedEventArgs(string propertyName, object? before, object? after)
            : base(propertyName)
        {
            this.Before = before;
            this.After = after;
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public object? Before { get; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public object? After { get; }
    }
}
