using System;
using System.Collections.Generic;
using System.Linq;

namespace SigCompiler.Emit
{
    public class SymbolTable
    {
        public int CurrentOffset { get; private set; }
        public int CurrentGlobalOffset { get; private set; }
        private Stack<Scope> scopes = new Stack<Scope>();
        private Scope globalScope = new Scope();

        public SymbolTable()
        {
            CurrentOffset = 0;
            CurrentGlobalOffset = 0;
            //scopes.Push(globalScope);
        }

        public int AddSymbol(string symbol, int size)
        {
            CurrentOffset += size;
            scopes.Peek().Symbols.Add(symbol, CurrentOffset);
            scopes.Peek().SymbolTypes.Add(symbol, size);
            return CurrentOffset;
        }

        public int AddGlobalSymbol(SourceLocation location, string symbol, int size)
        {
            symbol = location.File + symbol;

            CurrentGlobalOffset += size;
            globalScope.Symbols.Add(symbol, CurrentGlobalOffset);
            globalScope.SymbolTypes.Add(symbol, size);
            return CurrentGlobalOffset;
        }

        public bool ContainsSymbol(string symbol)
        {
            foreach (var scope in scopes)
                if (scope.Symbols.ContainsKey(symbol))
                    return true;
            return false;
        }

        public bool ContainsGlobalSymbol(SourceLocation location, string symbol)
        {
            return globalScope.Symbols.ContainsKey(location.File + symbol);
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

        public int GetGlobalOffset(SourceLocation location, string symbol)
        {
            symbol = location.File + symbol;

            if (!globalScope.Symbols.ContainsKey(symbol))
                throw new CompilerException(location, "Could not find global symbol {0} in the current scope!", symbol);

            return globalScope.Symbols[symbol];
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

        public int GetGlobalSize(SourceLocation location, string symbol)
        {
            symbol = location.File + symbol;

            if (!globalScope.Symbols.ContainsKey(symbol))
                throw new CompilerException(location, "Could not find global symbol {0} in the current scope!", symbol);

            return globalScope.SymbolTypes[symbol];
        }
        
        public void PushScope()
        {
            scopes.Push(new Scope());
        }

        public void PopScope()
        {
            scopes.Pop();
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