using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Unix.Probes.Process
{
    public class ProcessObjectCollector: BaseObjectCollector
    {
        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }
    }
}
