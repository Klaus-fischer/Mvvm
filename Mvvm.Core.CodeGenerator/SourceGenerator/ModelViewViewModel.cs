namespace Mvvm.Core.SourceGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [Generator]
    public class ModelViewViewModel : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is SyntaxReceiver receiver)
            {
                Debugger.Launch();

            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<PropertyDeclarationSyntax> Candidates { get; private set; }

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is PropertyDeclarationSyntax propertyDeclarationSyntax)
                {
                    foreach (var attributeList in propertyDeclarationSyntax.AttributeLists)
                    {
                        foreach (var attribute in attributeList.Attributes)
                        {
                            if (attribute.Name.ToString() == "MapTo" ||
                                attribute.Name.ToString() == nameof(MapToAttribute))
                            {
                                if (propertyDeclarationSyntax.AccessorList?.Accessors.Any(SyntaxKind.PublicKeyword) ?? false &&
                                    !(propertyDeclarationSyntax.AccessorList?.Accessors.Any(SyntaxKind.StaticKeyword ?? true))
                                {

                                }

                                this.Candidates.Add(propertyDeclarationSyntax);
                            }
                        }
                    }
                }
            }
        }
    }
}
