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
    /// <typeparam name="TProperty">Type of the property.</typeparam>
    internal class PropertyMonitor<T, TProperty> : IPropertyMonitor<TProperty>, IDisposable
        where T : INotifyPropertyChanged
    {
        private readonly T source;
        private readonly Func<T, TProperty> propertyGetter;

        private readonly Collection<Delegate> eventHandlers = new();

        private TProperty oldValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMonitor{T, TProperty}"/> class.
        /// </summary>
        /// <param name="source">View model to listen to.</param>
        /// <param name="propertyName">Name of the property to listen to.</param>
        /// <param name="propertyGetter">Getter to read the current value.</param>
        public PropertyMonitor(T source, string propertyName, Func<T, TProperty> propertyGetter)
        {
            this.source = source;
            this.propertyGetter = propertyGetter;
            this.PropertyName = propertyName;

            this.source.PropertyChanged += this.SourcePropertyChanged;
            this.oldValue = propertyGetter(source);
        }

        /// <inheritdoc/>
        public event EventHandler? Disposed;

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
        event EventHandler<OnPropertyChangedEventArgs<TProperty>>? IPropertyMonitor<TProperty>.PropertyChanged
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
            this.Disposed?.Invoke(this, EventArgs.Empty);
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.PropertyName)
            {
                var value = this.propertyGetter(this.source);
                var args = new OnPropertyChangedEventArgs<TProperty>(this.PropertyName, this.oldValue, value);
                this.oldValue = value;

                foreach (var handler in this.eventHandlers)
                {
                    if (handler is EventHandler eventHandler)
                    {
                        eventHandler(sender, args);
                    }

                    if (handler is EventHandler<OnPropertyChangedEventArgs<TProperty>> typeEventHandler)
                    {
                        typeEventHandler(sender, args);
                    }
                }
            }
        }
    }
}
