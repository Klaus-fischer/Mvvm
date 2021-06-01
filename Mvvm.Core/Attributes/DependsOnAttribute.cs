// <copyright file="DependsOnAttribute.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;

    /// <summary>
    /// Attribute definition to mark dependencies of properties or commands to view model properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DependsOnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property the target depends on.</param>
        /// <param name="propertyNames">Additional property names.</param>
        public DependsOnAttribute(string propertyName, params string[] propertyNames)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Empty string", nameof(propertyName));
            }

            this.PropertyNames = new string[propertyNames.Length + 1];
            this.PropertyNames[0] = propertyName;
            Array.Copy(propertyNames, 0, this.PropertyNames, 1, propertyNames.Length);
        }

        /// <summary>
        /// Gets the property names the target depends on.
        /// </summary>
        public string[] PropertyNames { get; }
    }
}
