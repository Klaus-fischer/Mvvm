// <copyright file="GeneratePropertyAttribute.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.CodeGeneration
{
    using System;

    /// <summary>
    /// Generates an property getter and setter to this field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class GeneratePropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratePropertyAttribute"/> class.
        /// </summary>
        /// <param name="setterVisibility">Property will have no getter.</param>
        public GeneratePropertyAttribute(SetterVisibility setterVisibility = SetterVisibility.Public)
        {
            this.SetterVisibility = setterVisibility;
        }

        /// <summary>
        /// Gets a value indicating whether property should be read-only.
        /// </summary>
        public SetterVisibility SetterVisibility { get; }
    }

    public enum SetterVisibility
    {
        Private,
        Protected,
        Internal,
        Public,
    }
}
