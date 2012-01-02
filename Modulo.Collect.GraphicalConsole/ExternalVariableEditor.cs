using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using System.Windows.Forms;

namespace Modulo.Collect.GraphicalConsole
{
    public abstract class ExternalVariableEditor
    {
        public ExternalVariableEditor(VariablesTypeVariableExternal_variable variable)
        {
            this.externalVariable = variable;
        }

        protected VariablesTypeVariableExternal_variable externalVariable;

        public abstract Control GetEditor(string value);
    }
}