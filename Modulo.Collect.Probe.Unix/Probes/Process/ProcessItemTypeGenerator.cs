using System.Collections.Generic;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Modulo.Collect.OVAL.Common;
using System;
using Modulo.Collect.OVAL.Common.comparators;



namespace Modulo.Collect.Probe.Unix.Probes.Process
{
    public class ProcessItemTypeGenerator: IItemTypeGenerator
    {
        public ProcessInfoCollector ProcessCollector { get; set; }

        public IEnumerable<OVAL.SystemCharacteristics.ItemType> GetItemsToCollect(ObjectType objectType, VariablesEvaluated variables)
        {
            var processObject = (OVAL.Definitions.Unix.process_object)objectType;
            var commandEntity = (EntityObjectStringType)processObject.Item;
            var commandEntityValues = new VariableEntityEvaluator(variables).EvaluateVariableForEntity(commandEntity);

            var itemsToCollect = new List<OVAL.SystemCharacteristics.ItemType>();
            foreach (var commandValue in commandEntityValues)
                itemsToCollect.AddRange(ProcessEntityOperation(commandValue, commandEntity));
            
            return itemsToCollect;
        }

        private IEnumerable<OVAL.SystemCharacteristics.ItemType> ProcessEntityOperation(String entityValue, EntityObjectStringType entity)
        {
            var allTargetProcesses = this.ProcessCollector.getProcessInfo();
            var processResult = new List<OVAL.SystemCharacteristics.ItemType>();
            var comparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);
            foreach (var targetProcess in allTargetProcesses)
            {
                if (comparator.Compare(targetProcess.Command, entityValue, entity.operation))
                {
                    var newProcessItem = CreateProcessItem(targetProcess);
                    processResult.Add(newProcessItem);
                }
            }

            return processResult;
        }

        private OVAL.SystemCharacteristics.Unix.process_item CreateProcessItem(UnixProcessInfo targetProcess)
        {
            return new OVAL.SystemCharacteristics.Unix.process_item()
            {
                command = OvalHelper.CreateItemEntityWithStringValue(targetProcess.Command),
                pid = OvalHelper.CreateItemEntityWithIntegerValue(targetProcess.Pid.ToString()),
                ppid = OvalHelper.CreateItemEntityWithIntegerValue(targetProcess.PPid.ToString()),
                priority = OvalHelper.CreateItemEntityWithIntegerValue(targetProcess.Prio.ToString()),
                tty = OvalHelper.CreateItemEntityWithStringValue(targetProcess.Tty),
                user_id = OvalHelper.CreateItemEntityWithIntegerValue(targetProcess.User),
                start_time = new OVAL.SystemCharacteristics.EntityItemStringType() { status = OVAL.SystemCharacteristics.StatusEnumeration.notcollected },
                exec_time = new OVAL.SystemCharacteristics.EntityItemStringType() { status = OVAL.SystemCharacteristics.StatusEnumeration.notcollected },
                ruid = new OVAL.SystemCharacteristics.EntityItemIntType() { status = OVAL.SystemCharacteristics.StatusEnumeration.notcollected },
                scheduling_class = new OVAL.SystemCharacteristics.EntityItemStringType() { status = OVAL.SystemCharacteristics.StatusEnumeration.notcollected }
            };
        }
    }
}
