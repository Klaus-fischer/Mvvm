// <copyright file="ModelProperty.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;

    public class MapToAttribute : Attribute
    {
        public MapToAttribute(string propertyName, string modelName = "Model")
        {
            this.PropertyName = propertyName;
            this.ModelName = modelName;
        }

        public string ModelName { get; }

        public string PropertyName { get; }
    }
}
