// <copyright file="ExpectedValuePropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    internal class ExpectedValuePropertyMonitor<TTarget, TProperty> : PropertyMonitor
        where TTarget : INotifyPropertyChanged
    {
        public readonly ITargetDescriptor<TTarget, TProperty> targetDiscriptor;
        private readonly TProperty expectedValue;

        public ExpectedValuePropertyMonitor(
            ITargetDescriptor<TTarget, TProperty> target,
            TProperty expectedValue)
            : base(target.Target, target.PropertyName)
        {
            this.targetDiscriptor = target;
            this.expectedValue = expectedValue;
        }


        /// <inheritdoc/>
        protected override bool BlockPropertyChangedInvocation()
        {
            var current = this.targetDiscriptor.GetCurrentValue();
            return !EqualityComparer<TProperty>.Default.Equals(this.expectedValue, current);
        }
    }
}
