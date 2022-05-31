namespace SIM.Mvvm.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    static class SymbolExtensions
    {
        public static string GetFullName(this ISymbol? symbol)
        {
            if (symbol is null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            if (symbol.ContainingNamespace is INamespaceSymbol @namespace)
            {
                sb.Append($"{@namespace}.");
            }

            sb.Append(symbol.Name);

            return sb.ToString();
        }

        public static AttributeData? GetAttribute(this ISymbol symbol, string attributeName)
        {
            return symbol.GetAttributes().FirstOrDefault(o => o.AttributeClass?.ToDisplayString() == attributeName);
        }

        public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, string attributeName)
        {
            return symbol.GetAttributes().Where(o => o.AttributeClass?.ToDisplayString() == attributeName);
        }
    }
}
