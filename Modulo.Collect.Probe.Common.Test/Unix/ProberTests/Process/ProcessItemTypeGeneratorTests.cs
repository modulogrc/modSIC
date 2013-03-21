using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Unix.Probes.Process;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Rhino.Mocks;
using Modulo.Collect.Probe.Unix.SSHCollectors;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.Process
{
    [TestClass]
    public class ProcessItemTypeGeneratorTests
    {
        private OVAL.Definitions.oval_definitions OvalDefinitionsSample;
        private List<UnixProcessInfo> FakeProcessInfoList;

        public ProcessItemTypeGeneratorTests()
        {
            this.OvalDefinitionsSample = ProbeHelper.GetFakeOvalDefinitions("definitions_all_unix.xml");
            
            var processInfo1 = new UnixProcessInfo() { Command = "http", Pid = 20, PPid = 0, Tty = "tty", User = "root" };
            var processInfo2 = new UnixProcessInfo() { Command = "ftp", Pid = 21, PPid = 20, Tty = "tty", User = "root" };
            var processInfo3 = new UnixProcessInfo() { Command = "ssh", Pid = 22, PPid = 0, Tty = "tty", User = "root" };
            var processInfo4 = new UnixProcessInfo() { Command = "named", Pid = 23, PPid = 0, Tty = "tty", User = "root" };
            

            this.FakeProcessInfoList = new UnixProcessInfo[] { processInfo1, processInfo2, processInfo3, processInfo4 }.ToList();
        }

        private OVAL.Definitions.ObjectType GetOvalObjectByID(string objectID)
        {
            return this.OvalDefinitionsSample.objects.First(obj => obj.id.Equals(objectID));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_for_a_simple_process_object()
        {
            var itemTypeGenerator = CreateItemTypeGeneratorWithBehavior();

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(GetOvalObjectByID("oval:modulo:obj:6"), null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(process_item));
            var processItem = (process_item)itemsToCollect.First();
            AssertProcessItem(processItem, "named", "23");

        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_for_a_process_object_with_referenced_variable()
        {
            var fakeVariableValues = new string[] { "ssh", "ftp" };
            var fakeVariables = VariableHelper.CreateVariableWithMultiplesValue("oval:modulo:obj:60", "oval:modulo:var:60", fakeVariableValues);

            var itemsToCollect =
                CreateItemTypeGeneratorWithBehavior()
                    .GetItemsToCollect(
                        GetOvalObjectByID("oval:modulo:obj:60"), fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 2, typeof(process_item));
            var firstProcessItem = (process_item)itemsToCollect.ElementAt(0);
            AssertProcessItem(firstProcessItem, "ssh", "22");
            var secondProcessItem = (process_item)itemsToCollect.ElementAt(1);
            AssertProcessItem(secondProcessItem, "ftp", "21", "20");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_not_equal_operation_to_process_object_command_entity()
        {
            var objectSample = GetOvalObjectByID("oval:modulo:obj:61");
            var itemTypeGenerator = CreateItemTypeGeneratorWithBehavior();
            
            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(objectSample, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 3, typeof(process_item));
            var firstProcessItem = (process_item)itemsToCollect.ElementAt(0);
            AssertProcessItem(firstProcessItem, "http", "20");
            var secondProcessItem = (process_item)itemsToCollect.ElementAt(1);
            AssertProcessItem(secondProcessItem, "ssh", "22");
            var thirdProcessItem = (process_item)itemsToCollect.ElementAt(2);
            AssertProcessItem(thirdProcessItem, "named", "23");
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_pattern_match_operation_to_process_object_command_entity()
        {
            var objectSample = GetOvalObjectByID("oval:modulo:obj:62");
            var itemTypeGenerator = CreateItemTypeGeneratorWithBehavior();

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(objectSample, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 2, typeof(process_item));
            var firstProcessItem = (process_item)itemsToCollect.ElementAt(0);
            AssertProcessItem(firstProcessItem, "http", "20");
            var secondProcessItem = (process_item)itemsToCollect.ElementAt(1);
            AssertProcessItem(secondProcessItem, "ftp", "21", "20");
        }

        private ProcessItemTypeGenerator CreateItemTypeGeneratorWithBehavior()
        {
            var mocks = new MockRepository();
            var fakeProcessCollector = mocks.DynamicMock<ProcessInfoCollector>(new object[] { null } );
            Expect.Call(fakeProcessCollector.GetProcessInfo()).Return(this.FakeProcessInfoList);
            mocks.ReplayAll();

            return new ProcessItemTypeGenerator() { ProcessCollector = fakeProcessCollector };
        }

        private void AssertProcessItem(process_item itemToAssert, string command, string pid, string ppid = "0", string tty = "tty", string userID = "root")
        {
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.command, command);
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.pid, pid);
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.ppid, ppid);
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.tty, tty);
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.user_id, userID);
        }
    }
}
