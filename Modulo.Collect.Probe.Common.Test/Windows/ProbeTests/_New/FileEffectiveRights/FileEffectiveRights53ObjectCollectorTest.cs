using Modulo.Collect.Probe.Windows.Probes._New.FileEffectiveRights53;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using System.Collections.Generic;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Windows.Test
{


    [TestClass]
    public class FileEffectiveRights53ObjectCollectorTest
    {

        [TestMethod, Ignore]
        public void CollectItemsTest()
        {
            var targetInfo = new TargetInfoFactory("10.1.0.176", "", "oval", "M0dul0-0v4l").Create();
            var wmiProvider = WmiDataProviderFactory.CreateWmiDataProviderForFileSearching(targetInfo);
            var objectCollector = new FileEffectiveRights53ObjectCollector(wmiProvider);


            var start = DateTime.UtcNow;
            var collectedItems = objectCollector.CollectItems("c:\\windows\\win.ini", ".*", OperationEnumeration.patternmatch);
            var end = DateTime.UtcNow;
            var time = end.Subtract(start).Seconds;

            var start1 = DateTime.UtcNow;
            var collectedItems1 = objectCollector.CollectItems("c:\\windows\\tsoc.log", ".*", OperationEnumeration.patternmatch);
            var end1 = DateTime.UtcNow;
            var time1 = end1.Subtract(start1).Seconds;

            var start2 = DateTime.UtcNow;
            var collectedItems2 = objectCollector.CollectItems("c:\\windows\\control.ini", ".*", OperationEnumeration.patternmatch);
            var end2 = DateTime.UtcNow;
            var time2 = end2.Subtract(start2).Seconds;
            
            Assert.Fail("{0} {1} {2}", time.ToString(), time1.ToString(), time2.ToString());
        }
    }
}
