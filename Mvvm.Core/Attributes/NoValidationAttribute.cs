// <copyright file="NoValidationAttribute.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
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
