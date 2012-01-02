using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using System.Windows.Forms;

namespace Modulo.Collect.GraphicalConsole
{
    class BinaryExternalVariableEditor : ExternalVariableEditor
    {
        public BinaryExternalVariableEditor(VariablesTypeVariableExternal_variable externalVariable)
            : base(externalVariable) { }

        public override Control GetEditor(string value)
        {
            IEnumerable<PossibleValueType> possibleValues = this.externalVariable.GetPossibleValues();
            TextBox textBox = new TextBox();
            textBox.Name = this.externalVariable.id;
            textBox.Text = value;
            return textBox;            
        }
    }
}
