using System;
using System.IO;

using SigCompiler.Emit;
using SigCompiler.Parser;
using SigCompiler.Scanner;

namespace SigCompiler
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                var ast = new SigParser(new PreprocessorHandler().ProcessFile(args[0])).Parse();
                Console.WriteLine(new Optimizer(new Compiler().Compile(ast)).Optimize());
            }
            catch (CompilerException ex)
            {
                Console.WriteLine("At {0}", ex.SourceLocation);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
            }
        }
    }
}
