// <copyright file="PropertyMonitor.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Factory to create a <see cref="IPropertyMonitor"/>.
    /// </summary>
    public class PropertyMonitorFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMonitor{T}"/> class.
        /// </summary>
        /// <param name="viewModel">View model to listen to.</param>
        /// <param name="propertyName">The name of the property the monitor listens to.</param>
        public static IPropertyMonitor Create(INotifyPropertyChanged viewModel, string propertyName)
        {
            var property = viewModel.GetType().GetProperty(propertyName);

            var propertyType = property.PropertyType;

            var value = Expression.Property(Expression.Constant(viewModel), propertyName);
            var lambda = Expression.Lambda(value);
            var getter = lambda.Compile();

            var propertyMonitorType_T = Type.GetType("SIM.Mvvm.PropertyMonitor`1")
                                            .MakeGenericType(propertyType);

            return (IPropertyMonitor)Activator.CreateInstance(
                propertyMonitorType_T,
                viewModel,
                propertyName,
                getter,
                null);
        }
    }
}
