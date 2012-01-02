using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.SystemCharacteristics.SapCode;
using Modulo.Collect.OVAL.Definitions.SapCode;
using Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.Probe.CodeControl
{
    public class SapCodeItemTypeGenerator : IItemTypeGenerator
    {
        public IEnumerable<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var sapcodeObjectType = (sapcode_object)objectType;
            var variableEvaluator = new VariableEntityEvaluator(variables);
            var issues = variableEvaluator.EvaluateVariableForEntity(((EntitySimpleBaseType)sapcodeObjectType.Items[sapcodeObjectType.ItemsElementName.ToList().IndexOf(SapCodeObjectItemsChoices.issue)]));
            var systemNames = variableEvaluator.EvaluateVariableForEntity(((EntitySimpleBaseType)sapcodeObjectType.Items[sapcodeObjectType.ItemsElementName.ToList().IndexOf(SapCodeObjectItemsChoices.system_name)]));
            var itemList = new List<ItemType>();
            foreach (var systemName in systemNames)
                foreach (var issueNumber in issues)
                    itemList.Add(new sapcode_item()
                    {
                        issue = OvalHelper.CreateItemEntityWithIntegerValue(issueNumber),
                        system_name = OvalHelper.CreateItemEntityWithIntegerValue(systemName)
                    });

            return itemList.ToArray();
        }
    }
}
