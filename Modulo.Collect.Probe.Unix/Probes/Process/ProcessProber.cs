using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Unix;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Unix.SSHCollectors;

namespace Modulo.Collect.Probe.Unix.Probes.Process
{
    [ProbeCapability(OvalObject = "process", PlataformName = FamilyEnumeration.unix)]
    public class ProcessProber : ProbeBase, IProbe
    {
        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            return null;
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            ConnectionProvider = ConnectionManager.Connect<SSHConnectionProvider>(connectionContext, target);
        }

        protected override void ConfigureObjectCollector()
        {
            if (this.ItemTypeGenerator == null)
            {
                var sshExec = ((SSHConnectionProvider)ConnectionProvider).SSHExec;
                this.ItemTypeGenerator = new ProcessItemTypeGenerator() { ProcessCollector = new ProcessInfoCollector(sshExec) };
            }

            if (this.ObjectCollector == null)
                this.ObjectCollector = new ProcessObjectCollector();

        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(
            IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<process_object>();
        }

        protected override OVAL.SystemCharacteristics.ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            var messageType = new MessageType() { level = MessageLevelEnumeration.error, Value = errorMessage };
            return new OVAL.SystemCharacteristics.Unix.process_item() { status = StatusEnumeration.error, message = new MessageType[] { messageType } };
        }
    }
}
