// <copyright file="NoValidationAttribute.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace Sim.Mvvm.Validation
{
    using System;

    /// <summary>
    /// Excludes a property from the validation in the <see cref="ValidationIDataErrorInfoModel"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NoValidationAttribute : Attribute
    {
    }
}
