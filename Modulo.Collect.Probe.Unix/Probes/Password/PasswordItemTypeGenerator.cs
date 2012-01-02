using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.OVAL.Definitions.Unix;
using Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.Probe.Unix.Probes.Password
{
    public class PasswordItemTypeGenerator: IItemTypeGenerator
    {
        public IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var usernameEntity = ((password_object)objectType).Items.OfType<EntityObjectStringType>().FirstOrDefault();
            if (usernameEntity != null)
            {
                var usernames = new VariableEntityEvaluator(variables).EvaluateVariableForEntity(usernameEntity);
                return 
                    usernames
                        .Select(
                            user =>
                                new password_item() { username = OvalHelper.CreateItemEntityWithStringValue(user) });
            }

            return null;
        }
    }
}
