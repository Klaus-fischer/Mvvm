// <copyright file="ChildPropertyChangedEventArgs.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
{
    using System.ComponentModel;

    /// <summary>
    /// Delegate for the <see cref="ITreeViewModel.ChildPropertyChanged"/> event.
    /// </summary>
    /// <param name="sender">Sender of the event.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void ChildPropertyChangedEventHandler(object sender, ChildPropertyChangedEventArgs args);

    /// <summary>
    /// Event arguments for <see cref="ITreeViewModel.ChildPropertyChanged"/> event.
    /// </summary>
    public class ChildPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="child">The model that contains the property that was changed.</param>
        /// <param name="propertyName">The name of the property that was changed.</param>
        public ChildPropertyChangedEventArgs(ITreeViewModel child, string? propertyName)
            : base(propertyName)
        {
            this.Child = child;
        }

        /// <summary>
        /// Gets the Child tree view model.
        /// </summary>
        public ITreeViewModel Child { get; }
    }
}
