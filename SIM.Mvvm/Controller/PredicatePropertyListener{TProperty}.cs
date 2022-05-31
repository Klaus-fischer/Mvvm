// <copyright file="PredicatePropertyListener{TProperty}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Wraps an <see cref="IPropertyListener"/> and filters events by condition.
    /// </summary>
    /// <typeparam name="TProperty">Type of the property to listen to.</typeparam>
    internal class PredicatePropertyMonitor<TProperty> : IPropertyMonitor<TProperty>, IDisposable
    {
        private readonly Collection<Delegate> eventHandlers = new();
        private readonly Func<TProperty, bool> predicate;
        private readonly IPropertyMonitor<TProperty> listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMonitor{TProperty}"/> class.
        /// </summary>
        /// <param name="listener">Listener to wrap.</param>
        /// <param name="predicate">Predicate to filter events.</param>
        public PredicatePropertyMonitor(
            IPropertyMonitor<TProperty> listener,
            Func<TProperty, bool> predicate)
        {
            this.listener = listener;
            this.predicate = predicate;

            this.listener.Disposed += this.OnListenerDisposing;
        }

        /// <inheritdoc/>
        public event EventHandler? Disposed;

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
        public string PropertyName => this.listener.PropertyName;

        /// <inheritdoc/>
        public void Dispose()
        {
            this.listener.PropertyChanged -= this.OnListenerPropertyChanged;
            if (this.listener is IDisposable disposable)
            {
                disposable.Dispose();
            }

            this.eventHandlers.Clear();
            this.Disposed?.Invoke(this, EventArgs.Empty);
        }

        private void OnListenerDisposing(object sender, EventArgs e)
        {
            this.listener.Disposed -= this.OnListenerDisposing;
            this.Dispose();
        }

        private void OnListenerPropertyChanged(object sender, OnPropertyChangedEventArgs<TProperty> args)
        {
            if (!this.predicate(args.After))
            {
                return;
            }

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
