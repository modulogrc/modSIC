using System;
using System.Linq;
using System.Collections.Generic;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions
{
    public class UniqueFunctionComponent : LocalVariableFunctionComponent
    {
        private UniqueFunctionType _uniqueFunctionType;

        public UniqueFunctionComponent(UniqueFunctionType uniqueFunctionType, 
            IEnumerable<VariableType> variablesOfDefinitions, oval_system_characteristics systemCharacteristics)
        {
            this._uniqueFunctionType = uniqueFunctionType;
        }

        public override IEnumerable<string> GetValue()
        {
            return components.SelectMany(x => x.GetValue()).Distinct();
        }
    }
}