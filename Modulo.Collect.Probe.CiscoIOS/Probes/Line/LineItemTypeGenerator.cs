using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Common.XmlSignatures;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics.Ios;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.Ios;

namespace Modulo.Collect.Probe.CiscoIOS.Probes.Line
{
    public class LineItemTypeGenerator: IItemTypeGenerator
    {
        public IEnumerable<OVAL.SystemCharacteristics.ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var showSubcommandEntity = ((line_object)objectType).GetShowSubcommandEntity();
            var showSubcommandValues = 
                new VariableEntityEvaluator(variables)
                    .EvaluateVariableForEntity(showSubcommandEntity);

            return 
                showSubcommandValues
                    .Select(v =>
                        new line_item() { show_subcommand = new EntityItemStringType() { Value = v } });
        }
    }
}
