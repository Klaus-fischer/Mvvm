// <copyright file="AutoMapPropertiesGenerator.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace SIM.Mvvm.CodeGenerator
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public partial class AutoMapPropertiesGenerator
    {
        /// <summary>
        /// Created on demand before each generation pass.
        /// </summary>
        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<ISymbol> Fields { get; } = new List<ISymbol>();

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
                            if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == AttributeFullName))
                            {
                                this.Fields.Add(fieldSymbol);
                            }
                        }
                    }
                }

                if (context.Node is PropertyDeclarationSyntax propertyDeclarationSyntax
                    && propertyDeclarationSyntax.AttributeLists.Count > 0)
                {
                    //if (context.Node.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PropertyDeclaration) &&
                    //    propertyDeclarationSyntax.ToFullString().Contains("DataObject"))
                    //{
                    //    Debugger.Launch();
                    //}

                    if (context.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax) is IPropertySymbol propertySymbol)
                    {
                        if (propertySymbol.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == AttributeFullName))
                        {
                            this.Fields.Add(propertySymbol);
                        }
                    }
                }
            }
        }
    }
}