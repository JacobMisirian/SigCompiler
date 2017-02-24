using System;

namespace SigCompiler.Emit
{
    public class Optimizer
    {
        private string asm;

        public Optimizer(string asm)
        {
            this.asm = asm;
        }

        public string Optimize()
        {
            removeRedundantStackInstructions();
            return asm;
        }

        private void removeRedundantStackInstructions()
        {
            asm = asm.Replace("push a\npop a\n", string.Empty);
            asm = asm.Replace("push b\npop b\n", string.Empty);
        }
    }
}

