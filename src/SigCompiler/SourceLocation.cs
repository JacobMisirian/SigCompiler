using System;

namespace SigCompiler
{
    public class SourceLocation
    {
        public string File { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }

        public SourceLocation(string file, int row, int column)
        {
            File = file;
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return string.Format("[SourceLocation: File={0}, Row={1}, Column={2}]", File, Row, Column);
        }
    }
}

