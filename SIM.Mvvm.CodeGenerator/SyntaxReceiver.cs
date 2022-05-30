// <copyright file="AutoMapPropertiesGenerator.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace SIM.Mvvm.CodeGeneration
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;


    /// <summary>
    /// Created on demand before each generation pass.
    /// </summary>
    internal class SyntaxReceiver : ISyntaxContextReceiver
    {
        public static readonly string GeneratePropertyAttributeFullName = typeof(GeneratePropertyAttribute).FullName;
        public static readonly string AutoMapPropertiesAttributeFullName = typeof(AutoMapPropertiesAttribute).FullName;

        public List<IFieldSymbol> FieldsToGeneratePropertiesFrom { get; } = new List<IFieldSymbol>();
        public List<ISymbol> FieldsToAutoMapProperties { get; } = new List<ISymbol>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            // any field with at least one attribute is a candidate for property generation
            if (context.Node is FieldDeclarationSyntax fieldDeclarationSyntax
                && fieldDeclarationSyntax.AttributeLists.Count > 0)
            {
                foreach (var variable in fieldDeclarationSyntax.Declaration.Variables)
                {
                    // Get the symbol being declared by the field, and keep it if its annotated
                    if (context.SemanticModel.GetDeclaredSymbol(variable) is IFieldSymbol fieldSymbol)
                    {
                        if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == GeneratePropertyAttributeFullName))
                        {
                            this.FieldsToGeneratePropertiesFrom.Add(fieldSymbol);
                        }

                        if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == AutoMapPropertiesAttributeFullName))
                        {
                            this.FieldsToAutoMapProperties.Add(fieldSymbol);
                        }
                    }
                }
            }

            if (context.Node is PropertyDeclarationSyntax propertyDeclarationSyntax
               && propertyDeclarationSyntax.AttributeLists.Count > 0)
            {
                if (context.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax) is IPropertySymbol propertySymbol)
                {
                    if (propertySymbol.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == AutoMapPropertiesAttributeFullName))
                    {
                        this.FieldsToAutoMapProperties.Add(propertySymbol);
                    }
                }
            }
        }
    }
}
