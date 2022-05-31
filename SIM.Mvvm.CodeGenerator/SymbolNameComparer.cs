namespace SIM.Mvvm.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class SymbolNameComparer : IEqualityComparer<ISymbol>
    {
        public bool Equals(ISymbol x, ISymbol y)
            => x.Name == y.Name;

        public int GetHashCode(ISymbol obj)
            => obj.Name.GetHashCode();
    }
}
