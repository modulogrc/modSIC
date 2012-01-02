using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Unix.Probes.Process;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Checkers;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.Process
{
    [TestClass]
    public class ProcessObjectCollectorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_process_item()
        {
            var objectCollector = new ProcessObjectCollector();

            var collectedItems =
                objectCollector
                    .CollectDataForSystemItem(
                        new OVAL.SystemCharacteristics.Unix.process_item()
                        {
                            command = OvalHelper.CreateItemEntityWithStringValue("ssh"),
                            pid = OvalHelper.CreateItemEntityWithIntegerValue("30"),
                            ppid = OvalHelper.CreateItemEntityWithIntegerValue("0")
                        });

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(OVAL.SystemCharacteristics.Unix.process_item), true);
            var collectedProcessItem = (OVAL.SystemCharacteristics.Unix.process_item)collectedItems.First().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedProcessItem.command, "ssh");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedProcessItem.pid, "30");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedProcessItem.ppid, "0");
        }
    }
}
