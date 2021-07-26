// <copyright file="CallOnPropertyChangedAttribute.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;

    /// <summary>
    /// Attribute to mark methods that should be called on property changed.
    /// Methods must be delegates of type <see cref="Action"/> or <see cref="EventHandler{AdvancedPropertyChangedEventArgs}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallOnPropertyChangedAttribute : DependsOnAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallOnPropertyChangedAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property to listen to.</param>
        /// <param name="propertyNames">Additional property names.</param>
        public CallOnPropertyChangedAttribute(string propertyName, params string[] propertyNames)
            : base(propertyName, propertyNames)
        {
        }
    }
}
