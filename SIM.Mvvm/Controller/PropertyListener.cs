// <copyright file="PropertyListener.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Listens to an view model and raises an event on property changed.
    /// </summary>
    /// <typeparam name="T">Type of the view model.</typeparam>
    internal class PropertyListener<T> : IPropertyListener, IDisposable
        where T : INotifyPropertyChanged
    {
        private readonly T source;

        private readonly Collection<EventHandler> eventHandlers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyListener{T}"/> class.
        /// </summary>
        /// <param name="source">View model to listen to.</param>
        /// <param name="propertyName">Name of the property to listen to.</param>
        public PropertyListener(T source, string propertyName)
        {
            this.source = source;
            this.PropertyName = propertyName;

            this.source.PropertyChanged += this.SourcePropertyChanged;
        }

        /// <inheritdoc/>
        event EventHandler? IPropertyListener.PropertyChanged
        {
            add
            {
                if (value is not null)
                {
                    this.eventHandlers.Add(value);
                }
            }

            remove
            {
                if (value is not null)
                {
                    this.eventHandlers.Remove(value);
                }
            }
        }

        /// <inheritdoc/>
        public string PropertyName { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.source.PropertyChanged -= this.SourcePropertyChanged;
            this.eventHandlers.Clear();
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.PropertyName)
            {
                foreach (var handler in this.eventHandlers)
                {
                    handler(sender, EventArgs.Empty);
                }
            }
        }
    }
}
