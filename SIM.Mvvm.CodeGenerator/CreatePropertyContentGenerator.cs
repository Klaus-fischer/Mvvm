namespace SIM.Mvvm.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    internal class CreatePropertyContentGenerator : IContentGenerator
    {
        private const string attributeFullName = "SIM.Mvvm.CodeGeneration.CreatePropertyAttribute";
        private const string attributeContext = @"
using System;

namespace SIM.Mvvm.CodeGeneration
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class, AllowMultiple = true)]
    internal class CreatePropertyAttribute : Attribute
    {
        public CreatePropertyAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
";
        private Dictionary<ISymbol, List<string>> propertiesToCreate { get; } =
            new Dictionary<ISymbol, List<string>>(comparer: new SymbolNameComparer());

        public void PostInititialize(GeneratorPostInitializationContext context)
        {
            context.AddSource("CreatePropertyAttribute.g.cs", attributeContext);
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (!this.TryGetSymbol(context, out var symbol))
            {
                return;
            }

            if (!this.propertiesToCreate.TryGetValue(symbol, out var collection))
            {
                collection = new List<string>();
                this.propertiesToCreate.Add(symbol, collection);
            }

            foreach (var attribute in symbol.GetAttributes(attributeFullName))
            {
                if (attribute.ConstructorArguments.FirstOrDefault().Value is string propertyName)
                {
                    if (!collection.Contains(propertyName))
                    {
                        collection.Add(propertyName);
                    }
                }
            }
        }

        private bool TryGetSymbol(GeneratorSyntaxContext context, out ISymbol symbol)
        {
            if (context.Node is ClassDeclarationSyntax classDeclarationSyntax &&
               classDeclarationSyntax.AttributeLists.Count > 0)
            {
                if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is ISymbol candidate)
                {
                    if (candidate.GetAttribute(attributeFullName) is not null)
                    {
                        symbol = candidate;
                        return true;
                    }
                }
            }

            if (context.Node is ConstructorDeclarationSyntax ctorDeclarationSyntax &&
               ctorDeclarationSyntax.AttributeLists.Count > 0)
            {
                if (context.SemanticModel.GetDeclaredSymbol(ctorDeclarationSyntax) is ISymbol candidate)
                {
                    if (candidate.GetAttribute(attributeFullName) is not null)
                    {
                        symbol = candidate;
                        return true;
                    }
                }
            }

            symbol = default;
            return false;
        }

        public void Execute(GeneratorExecutionContext context)
        {
            foreach (var className in this.propertiesToCreate.Keys)
            {
                var content = this.GenerateClass(className, this.propertiesToCreate[className]);

                context.AddSource($"{className}.GeneratedProperties.cs", content);
            }
        }

        private string GenerateClass(ISymbol symbol, List<string> list)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($@"
namespace {symbol.ContainingNamespace}
{{
    partial class {symbol.Name} 
    {{
");
            foreach (var item in list)
            {
                sb.AppendLine($@"
        public string {item} {{ get; set; }}
");
            }

            sb.AppendLine($@"
    }}
}}");

            return sb.ToString();
        }

        private (string Namespace, string ClassName) Parse(string fullName)
        {
            var match = Regex.Match(fullName, @"(.*)\.([^\.]+)");

            return (match.Groups[1].Value, match.Groups[2].Value);
        }
    }
}
