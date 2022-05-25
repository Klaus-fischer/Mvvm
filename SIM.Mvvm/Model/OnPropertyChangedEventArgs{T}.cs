// <copyright file="EventArgs{T}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;

    public class OnPropertyChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnPropertyChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="before">The value before the change.</param>
        /// <param name="after">The value after the change.</param>
        public OnPropertyChangedEventArgs(string propertyName, T before, T after)
        {
            this.PropertyName = propertyName;
            this.Before = before;
            this.After = after;
        }

        /// <summary>
        /// Gets the name of the property that was changed.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the value before the change.
        /// </summary>
        public T Before { get; }

        /// <summary>
        /// Gets the value after the change.
        /// </summary>
        public T After { get; }
    }
}
