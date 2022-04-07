// <copyright file="TargetDescriptor{TTarget,TProperty}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.ComponentModel;

    internal class TargetDescriptor<TTarget, TProperty> : ITargetDescriptor<TTarget, TProperty>
        where TTarget : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetDescriptor{TTarget, TProperty}"/> class.
        /// </summary>
        /// <param name="viewModel">View model to listen to.</param>
        /// <param name="propertyName">The name of the property the monitor listens to.</param>
        /// <param name="getter">The callback to get the </param>
        public TargetDescriptor(TTarget viewModel, string propertyName, Func<TTarget, TProperty> getter)
        {
            this.Target = viewModel;
            this.PropertyName = propertyName;
            this.Getter = getter;
        }

        /// <inheritdoc/>
        public Func<TTarget, TProperty> Getter { get; }

        /// <inheritdoc/>
        public string PropertyName { get; }

        /// <inheritdoc/>
        public TTarget Target { get; }

        /// <inheritdoc/>
        public TProperty GetCurrentValue() =>
            this.Getter(this.Target);
    }
}
