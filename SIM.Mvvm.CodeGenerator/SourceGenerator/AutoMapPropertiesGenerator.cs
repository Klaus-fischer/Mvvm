// <copyright file="AutoMapPropertiesGenerator.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    [Generator]
    public class AutoMapPropertiesGenerator : ISourceGenerator
    {
        private static readonly string AttributeFullName = typeof(AutoMapPropertiesAttribute).FullName;
        private static readonly string ViewModelFullName = typeof(BaseViewModel).FullName;

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif

            // retrieve the populated receiver
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
            {
                return;
            }

            // get the added attribute, and INotifyPropertyChanged
            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName(AttributeFullName)
                ?? throw new ArgumentNullException();

            INamedTypeSymbol baseViewModelClassSymbol = context.Compilation.GetTypeByMetadataName(ViewModelFullName)
                ?? throw new ArgumentNullException();

            // group the fields by class, and generate the source
            foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in receiver.Fields.GroupBy(f => f.ContainingType))
            {
                string classSource = this.ProcessClass(group.Key, group.ToList(), attributeSymbol, baseViewModelClassSymbol, context);
                context.AddSource($"{group.Key.Name}_autoMap.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private string ProcessClass(
            INamedTypeSymbol classSymbol,
            List<IFieldSymbol> fields,
            ISymbol attributeSymbol,
            ISymbol baseViewModelClassSymbol,
            GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return string.Empty;
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            if (this.CheckDerived(classSymbol, baseViewModelClassSymbol))
            {
                return string.Empty;
            }

            // begin building the generated source
            StringBuilder source = new StringBuilder($@"
namespace {namespaceName}
{{
    public partial class {classSymbol.Name}
    {{
");
            var knownProperties = classSymbol.GetMembers()
                .Where(o => o.Kind == SymbolKind.Property || o.Kind == SymbolKind.Field)
                .Select(o => o.Name)
                .ToList();

            // create properties for each field
            foreach (IFieldSymbol fieldSymbol in fields)
            {
                foreach (IPropertySymbol propertySymbol in fieldSymbol.Type.GetMembers().Where(o =>
                    o.Kind == SymbolKind.Property &&
                    o.DeclaredAccessibility == Accessibility.Public &&
                    !o.IsStatic).OfType<IPropertySymbol>())
                {
                    if (knownProperties.Contains(propertySymbol.Name))
                    {
                        Debug.WriteLine($"Skipping '{propertySymbol.Name}' (already defined).");
                        continue;
                    }

                    knownProperties.Add(propertySymbol.Name);

                    this.ProcessProperty(source, propertySymbol, fieldSymbol.Name, attributeSymbol);
                }

            }

            source.Append("} }");
            return source.ToString();
        }

        private bool CheckDerived(INamedTypeSymbol? classSymbol, ISymbol baseClassSymbol)
        {
            while (!SymbolEqualityComparer.Default.Equals(classSymbol, baseClassSymbol))
            {
                classSymbol = classSymbol?.BaseType;

                if (classSymbol is null)
                {
                    return false;
                }
            }

            return true;
        }

        private void ProcessProperty(StringBuilder source, IPropertySymbol propertySymbol, string fieldName, ISymbol attributeSymbol)
        {
            // get the name and type of the field
            string propertyName = propertySymbol.Name;
            ITypeSymbol fieldType = propertySymbol.Type;

            if (propertyName.Length == 0)
            {
                return;
            }

            var indicator = char.ToLower(fieldName[0]);

            source.Append($@"
public {fieldType} {propertyName} 
{{
    get 
    {{
        return this.{fieldName}.{propertyName};
    }}
    set
    {{
        this.SetPropertyValue(this.{fieldName}, {indicator} => {indicator}.{propertyName}, value);
    }}
}}
");
        }

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
                    foreach (VariableDeclaratorSyntax variable in fieldDeclarationSyntax.Declaration.Variables)
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
                   // this.Fields.Add(propertyDeclarationSyntax.)
                }
            }
        }
    }
}