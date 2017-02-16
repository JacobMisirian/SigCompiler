using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SigCompiler.Scanner
{
    public class PreprocessorHandler
    {
        private const string EXTENSION = ".sg";
        private const string FOLDER = "Signal";

        private string code;
        private string[] lines;

        private List<Token> tokens;

        public List<Token> ProcessFile(string file)
        {
            code = File.ReadAllText(file);
            lines = code.Split('\n');

            tokens = new List<Token>();

            foreach (string line in lines)
                if (line.Trim().StartsWith("#"))
                    scanPreprocessor(line.Trim().Substring(1));

            foreach (var token in new Lexer().Scan(file, code))
                tokens.Add(token);

            return tokens;
        }
        
        private List<string> files = new List<string>();
        private void scanPreprocessor(string line)
        {
            string[] parts = line.Split(' ');
            switch (parts[0].ToLower())
            {
                case "define":
                    code = code.Replace(parts[1], string.Join(" ", parts, 2, parts.Length - 2));
                    break;
                case "include":
                    string filePath = findFile(parts[1]);
                    if (files.Contains(filePath))
                        break;
                    files.Add(filePath);
                    if (!File.Exists(filePath))
                        throw new CompilerException(new SourceLocation(parts[1], 0, 0), "Could not find file {0}!", parts[1]);
                    foreach (var token in new PreprocessorHandler().ProcessFile(filePath))
                        tokens.Add(token);
                    break;
            }
        }

        private string findFile(string file)
        {
            string fileWithExtension = string.Format("{0}.{1}", file, EXTENSION);
            string fileInHome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), file);
            string fileInHomeWithExtension = string.Format("{0}.{1}", fileInHome, EXTENSION);
            string fileInHomeSig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SigCompiler", "lib", file);
            string fileInHomeSigWithExtension = string.Format("{0}.{1}", fileInHomeSig, EXTENSION);

            if (File.Exists(fileWithExtension))
                return fileWithExtension;
            if (File.Exists(fileInHome))
                return fileInHome;
            if (File.Exists(fileInHomeWithExtension))
                return fileInHomeWithExtension;
            if (File.Exists(fileInHomeSig))
                return fileInHomeSig;
            if (File.Exists(fileInHomeSigWithExtension))
                return fileInHomeSigWithExtension;
            return file;
        }
    }
}