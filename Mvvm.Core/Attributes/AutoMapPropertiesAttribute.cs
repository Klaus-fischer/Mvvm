// <copyright file="AutoMapPropertiesAttribute.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core.CodeGeneration
{
    using System;

    /// <summary>
    /// This attribute is used to mark model fields or properties to auto generate properties that points to the field.
    /// The attribute is only valid if you are using the <see cref="Mvvm.Core.CodeGeneration"/> project.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class AutoMapPropertiesAttribute : Attribute
    {
        public AutoMapPropertiesAttribute()
        {
        }

        public string[] Except { get; }
    }
}