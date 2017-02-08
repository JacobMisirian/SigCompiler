using System;
using System.Collections.Generic;

namespace SigCompiler.Emit
{
    public class SymbolTable
    {
        public int CurrentOffset { get; private set; }
        private Stack<Scope> scopes = new Stack<Scope>();
        private Scope globalScope = new Scope();

        public SymbolTable()
        {
            CurrentOffset = 0;
            scopes.Push(globalScope);
        }

        public int AddSymbol(string symbol, int size)
        {
            CurrentOffset += size;
            scopes.Peek().Symbols.Add(symbol, CurrentOffset);
            scopes.Peek().SymbolTypes.Add(symbol, size);
            return CurrentOffset;
        }

        public void AddGlobalSymbol(string symbol, int size)
        {
            globalScope.Symbols.Add(symbol, 0);
            globalScope.SymbolTypes.Add(symbol, size);
        }

        public bool ContainsSymbol(string symbol)
        {
            foreach (var scope in scopes)
                if (scope.Symbols.ContainsKey(symbol))
                    return true;
            return false;
        }

        public int GetOffset(SourceLocation location, string symbol)
        {
            if (!ContainsSymbol(symbol))
                throw new CompilerException(location, "Could not find symbol {0} in the current scope!", symbol);

            foreach (var scope in scopes)
                if (scope.Symbols.ContainsKey(symbol))
                    return scope.Symbols[symbol];
            return -1;
        }

        public int GetSize(SourceLocation location, string symbol)
        {
            if (!ContainsSymbol(symbol))
                throw new CompilerException(location, "Could not find symbol {0} in the current scope!", symbol);

            foreach (var scope in scopes)
                if (scope.SymbolTypes.ContainsKey(symbol))
                    return scope.SymbolTypes[symbol];
            return -1;
        }

        public void PushScope()
        {
            scopes.Push(new Scope());
        }

        public void PopScope()
        {
            scopes.Pop();
            if (scopes.Count <= 1)
                CurrentOffset = 0;
        }

        private class Scope
        {
            public Dictionary<string, int> Symbols { get; private set; }
            public Dictionary<string, int> SymbolTypes { get; private set; }

            public Scope()
            {
                Symbols = new Dictionary<string, int>();
                SymbolTypes = new Dictionary<string, int>();
            }
        }
    }
}