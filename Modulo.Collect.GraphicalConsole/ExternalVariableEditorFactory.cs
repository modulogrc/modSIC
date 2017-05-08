using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.GraphicalConsole
{
    public class ExternalVariableEditorFactory
    {
        public ExternalVariableEditor CreateExternalVariableEditor(VariablesTypeVariableExternal_variable externalVariable)
        {
            switch (externalVariable.datatype)
            {
                case SimpleDatatypeEnumeration.@string:
                case SimpleDatatypeEnumeration.version:
                    return new StringExternalVariableEditor(externalVariable);

                case SimpleDatatypeEnumeration.@int:
                    return new IntExternalVariableEditor(externalVariable);

                case SimpleDatatypeEnumeration.boolean:
                    return new BooleanExternalVariableEditor(externalVariable);

                case SimpleDatatypeEnumeration.binary:
                    return new BinaryExternalVariableEditor(externalVariable);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
