using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Unix.Password;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Modulo.Collect.OVAL.Definitions.Unix;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Unix.Probes.Password
{
    [ProbeCapability(OvalObject="password", PlataformName=FamilyEnumeration.unix)]
    public class PasswordProber: ProbeBase, IProbe
    {
        protected override OVAL.Definitions.set GetSetElement(OVAL.Definitions.ObjectType objectType)
        {
            return objectType.GetEntityBaseTypes().OfType<OVAL.Definitions.set>().SingleOrDefault();
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            base.ConnectionProvider = ConnectionManager.Connect<SSHConnectionProvider>(connectionContext, target);
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                var newItemsCollector = new PasswordCollector() { SSHExec = ((SSHConnectionProvider)ConnectionProvider).SSHExec };
                ObjectCollector = new PasswordObjectCollector() { PasswordItemsCollector = newItemsCollector };
                ItemTypeGenerator = new PasswordItemTypeGenerator();
            }
        }

        protected override IEnumerable<OVAL.Definitions.ObjectType> GetObjectsOfType(IEnumerable<OVAL.Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<password_object>();
        }

        protected override OVAL.SystemCharacteristics.ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new password_item() 
            { 
                status = StatusEnumeration.error, 
                message = new[] { new MessageType() { level = MessageLevelEnumeration.error, Value = errorMessage } } 
            };
        }
    }
}
