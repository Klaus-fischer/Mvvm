// <copyright file="ITargetDescriptor{TTarget,TProperty}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.ComponentModel;

    public interface ITargetDescriptor<TTarget, TProperty>
        where TTarget : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the callback to receive the current value.
        /// </summary>
        Func<TTarget, TProperty> Getter { get; }

        /// <summary>
        /// Gets the name of the property that will be monitored.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Gets the reference to the target to observe.
        /// </summary>
        TTarget Target { get; }

        /// <summary>
        /// Gets the current value of the monitored property.
        /// </summary>
        /// <returns>The current Value.</returns>
        TProperty GetCurrentValue();
    }
}