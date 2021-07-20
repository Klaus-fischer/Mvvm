﻿// <copyright file="AutoMapPropertiesGenerator.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace SIM.Mvvm.CodeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    [Generator]
    public partial class AutoMapPropertiesGenerator : ISourceGenerator
    {
        private static readonly string AttributeFullName = "SIM.Mvvm.CodeGeneration.AutoMapPropertiesAttribute";
        private static readonly string ViewModelFullName = "SIM.Mvvm.ViewModel";

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver
            if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            // get the added attribute, and INotifyPropertyChanged
            var attributeSymbol = context.Compilation.GetTypeByMetadataName(AttributeFullName)
               ?? throw new ArgumentNullException();

            // group the fields by class, and generate the source
            foreach (var group in receiver.Fields.GroupBy(f => f.ContainingType))
            {
                var classSource = this.ProcessClass(group.Key, group.ToList());
                context.AddSource($"{group.Key.Name}_autoMap.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private string ProcessClass(
            INamedTypeSymbol classSymbol,
            List<ISymbol> fields)
        {
            //System.Diagnostics.Debugger.Launch();

            if (!this.CheckClass(classSymbol, out string namespaceName))
            {
                return string.Empty;
            }

            // begin building the generated source
            var writer = new StringBuilder($@"
namespace {namespaceName}
{{
    public partial class {classSymbol.Name}
    {{");

            foreach (var property in this.GetAutoGeneratedProperties(classSymbol, fields, writer))
            {
                writer.Append($@"
        /// <summary>
        /// Gets or sets the <see cref=""{property.ModelType}.{property.PropertyName}""/> property of the <see cref=""{property.ModelName}""/> model.
        /// </summary>
        public {property.PropertyType} {property.PropertyName} 
        {{
            get => this.{property.ModelName}.{property.PropertyName};
            set => this.SetPropertyValue(() => this.{property.ModelName}.{property.PropertyName}, value);
        }}
");
            }

            writer.Append($@"    }}
}}");
            return writer.ToString();
        }

        private bool CheckClass(
            INamedTypeSymbol classSymbol,
            out string @namespace)
        {
            @namespace = string.Empty;

            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return false;
            }

            if (!this.CheckDerived(classSymbol))
            {
                return false; ;
            }

            @namespace = classSymbol.ContainingNamespace.ToDisplayString();
            return true;
        }

        private bool CheckDerived(INamedTypeSymbol? classSymbol)
        {
            while (classSymbol.GetFullName() != ViewModelFullName)
            {
                classSymbol = classSymbol?.BaseType;

                if (classSymbol is null)
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<AutoGeneratedProperty> GetAutoGeneratedProperties(
            INamedTypeSymbol classSymbol,
            List<ISymbol> modelCollection,
            StringBuilder writer)
        {
            var knownProperties = classSymbol.GetMembers()
                .Where(o => o.Kind == SymbolKind.Property || o.Kind == SymbolKind.Field)
                .Select(o => o.Name)
                .ToList();

            // create properties for each field
            foreach (var model in modelCollection)
            {
                if (!this.TryGetSymbolType(model, out var type))
                {
                    continue;
                }

                var excludedProperties = this.GetExcludedProperties(model);

                foreach (var modelProperty in type.GetMembers().Where(o =>
                    o.Kind == SymbolKind.Property &&
                    o.DeclaredAccessibility == Accessibility.Public && !o.IsStatic)
                    .OfType<IPropertySymbol>())
                {
                    if (!this.CheckExclusion(modelProperty, excludedProperties, knownProperties, out string message))
                    {
                        writer.Append($@"
        // {model.Name}.{modelProperty?.Name} {message}
");
                        continue;
                    }

                    yield return new AutoGeneratedProperty()
                    {
                        PropertyName = modelProperty.Name,
                        PropertyType = modelProperty.Type.Name,
                        ModelName = model.Name,
                        ModelType = type.Name,
                    };
                }
            }
        }

        private bool TryGetSymbolType(ISymbol symbol, out ITypeSymbol type)
        {
            if (symbol is IFieldSymbol fieldSymbol)
            {
                type = fieldSymbol.Type;
            }
            else if (symbol is IPropertySymbol property)
            {
                type = property.Type;
            }
            else
            {
                type = symbol.ContainingType;
                return false;
            }

            return true;
        }

        private string[] GetExcludedProperties(ISymbol symbol)
        {
            var attribute = symbol.GetAttributes()
                .First(ad => ad.AttributeClass?.ToDisplayString() == AttributeFullName);

            if (attribute is AttributeData ad)
            {
                if (ad.ConstructorArguments.Any())
                {
                    return ad.ConstructorArguments.SelectMany(o => o.Values).Select(o => o.Value).OfType<string>().ToArray();
                }
            }

            return Array.Empty<string>();
        }

        private bool CheckExclusion(IPropertySymbol? propertySymbol, IEnumerable<string> excludedProperties, ICollection<string> knownProperties, out string message)
        {
            if (propertySymbol is null)
            {
                message = "symbol is null";
                return false;
            }

            if (excludedProperties.Contains(propertySymbol.Name))
            {
                message = $"skipped (Excluded).";
                return false;
            }

            if (knownProperties.Contains(propertySymbol.Name))
            {
                message = $"skipped (already defined).";
                return false;
            }

            knownProperties.Add(propertySymbol.Name);
            message = "";
            return true;
        }
    }
}