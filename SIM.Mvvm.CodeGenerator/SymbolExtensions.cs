namespace SIM.Mvvm.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using System.Text;

    static class SymbolExtensions
    {
        public static string? GetFullName(this INamedTypeSymbol? symbol)
        {
            if (symbol is null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            if (symbol.ContainingNamespace is INamespaceSymbol @namespace)
            {
                sb.Append($"{@namespace}.");
            }

            sb.Append(symbol.Name);

            return sb.ToString();
        }
    }
}
