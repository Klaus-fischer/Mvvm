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
    internal class GenerateProperty1Attribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateProperty1Attribute"/> class.
        /// </summary>
        /// <param name="setterVisibility">Property will have no getter.</param>
        public GenerateProperty1Attribute(SetterVisibility setterVisibility = SetterVisibility.Public)
        {
            this.SetterVisibility = setterVisibility;
        }

        /// <summary>
        /// Gets a value indicating whether property should be read-only.
        /// </summary>
        public SetterVisibility SetterVisibility { get; }
    }

    internal enum SetterVisibility
    {
        Private,
        Protected,
        Internal,
        Public,
    }
}
