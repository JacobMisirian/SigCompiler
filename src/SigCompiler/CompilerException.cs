using System;

namespace SigCompiler
{
    public class CompilerException : Exception
    {
        public new string Message { get; private set; }
        public SourceLocation SourceLocation { get; private set; }

        public CompilerException(SourceLocation location, string msgf, params object[] args)
        {
            Message = string.Format(msgf, args);
            SourceLocation = location;
        }
    }
}

