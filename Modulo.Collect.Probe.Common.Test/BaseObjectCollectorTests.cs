using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.Probes.WMI.Wmi57;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;

namespace Modulo.Collect.Probe.Common.Test
{
    [TestClass]
    public class BaseObjectCollectorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void If_an_unexpected_error_occurs_during_object_collection_the_execution_log_must_contains_a_warning_level_entry()
        {
            var wmiObjectCollector = new Wmi57ObjectCollector(null) { WmiDataProvider = NewFakeWmiProvider() };

            var collectedItems = wmiObjectCollector.CollectDataForSystemItem(NewFakeWmi57Item());

            var executionLogs = collectedItems.First().ExecutionLog;
            Assert.IsNull(executionLogs.FirstOrDefault(log => log.Type == TypeItemLog.Error), "Error log entries were found.");
            var warningLogEntry = executionLogs.FirstOrDefault(log => log.Type == TypeItemLog.Warning);
            Assert.IsNotNull(warningLogEntry, "No warning log entry was found.");
            Assert.IsTrue(warningLogEntry.Message.Contains("Test Exception"));
        }

        private WmiDataProvider NewFakeWmiProvider()
        {
            var mocks = new MockRepository();
            var fakeWmiProvider = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(fakeWmiProvider.ExecuteWQL(null)).IgnoreArguments().Throw(new Exception("Test Exception"));
            mocks.ReplayAll();

            return fakeWmiProvider;
        }

        private ItemType NewFakeWmi57Item()
        {
            return new wmi57_item()
            {
                @namespace = OvalHelper.CreateItemEntityWithStringValue("root\\cimv2"),
                wql = OvalHelper.CreateItemEntityWithStringValue("Select * from Win32_OperatingSystem")
            };
        }
    
    }
}
