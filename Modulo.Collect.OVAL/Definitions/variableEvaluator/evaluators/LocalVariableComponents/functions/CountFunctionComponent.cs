using System;
using System.Linq;
using System.Collections.Generic;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions
{
    public class CountFunctionComponent : LocalVariableFunctionComponent
    {
        private CountFunctionType _countFunctionType;

        public CountFunctionComponent(CountFunctionType countFunctionType, 
            IEnumerable<VariableType> variablesOfDefinitions, oval_system_characteristics systemCharacteristics)
        {
            this._countFunctionType = countFunctionType;
        }

        public override IEnumerable<string> GetValue()
        {
            return new []{components.SelectMany(x=>x.GetValue()).Count().ToString()};
        }
    }
}