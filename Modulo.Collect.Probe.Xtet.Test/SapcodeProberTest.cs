using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.SapCode;
using Modulo.Collect.OVAL.Plugins;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics.SapCode;
using System.Linq;
using Modulo.Collect.Probe.CodeControl;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass]
    public class SapcodeProberTest 
    {
        protected TargetInfo FakeTargetInfo;
        protected List<IConnectionProvider> FakeContext;

        public SapcodeProberTest()
        {
            FakeTargetInfo = ProbeHelper.CreateFakeTarget();
            FakeContext = ProbeHelper.CreateFakeContext();
        }
      

        //[TestMethod, Owner("mgaspar")]
        //public void Should_be_possible_to_Load_the_sapcode_probe()
        //{
        //    PluginContainer.RegisterOvalAssembly(typeof (sapcode_object).Assembly);
        //    PluginContainer.RegisterProbeAssembly(typeof(SapCodeProberWindows).Assembly);
        //    var def = GetFakeOvalDefinitions("definitions_all_sapcode.xml");
        //    // Arrange
        //    var probeManager = new ProbeManager();
        //    var probes = probeManager.GetProbesFor(def.objects, FamilyEnumeration.undefined);

        //    Assert.AreEqual(1, probes.Count());
        //    Assert.AreEqual("sapcode", probes.Select(x=>x.Capability.OvalObject).First());
        //}

        [TestMethod, Owner("mgaspar"), Ignore]
        public void Should_be_possible_to_collect_sapcode_objects()
        {
            PluginContainer.RegisterOvalAssembly(typeof(sapcode_object).Assembly);
            var def = GetFakeOvalDefinitions("definitions_all_sapcode.xml");
            // Arrange

            var sapProber = new SapCodeProberWindows();

            FakeTargetInfo["HostName"] = "http://174.143.211.210:8080/CodeControlWeb/services/ScanWS";
            FakeTargetInfo.credentials = new Credentials("","modulo","modulo01", null);
            // Act
            var result = sapProber.Execute(FakeContext, FakeTargetInfo, ProbeHelper.CreateFakeCollectInfo(def.objects, null, null));

            // Assert
            // DoAssertForSingleCollectedObject(result, typeof(sapcode_item));
            Assert.IsTrue(Convert.ToInt32(((sapcode_item)result.CollectedObjects.First().SystemData[0]).total_issues_found.Value) > 0);
            Assert.AreEqual("0", ((sapcode_item)result.CollectedObjects.Last().SystemData[0]).total_issues_found.Value);
        }

        public oval_definitions GetFakeOvalDefinitions(string fileName)
        {
            if (!fileName.Contains(".xml"))
                fileName += ".xml";

            var sampleDoc = GetStreamFrom(fileName, "Resources");

            IEnumerable<string> errors;
            return oval_definitions.GetOvalDefinitionsFromStream(sampleDoc, out errors);
        }
        private System.IO.Stream GetStreamFrom(string fileNameOnly,string directoryName)
        {
            var assemblyName = GetType().Assembly.GetName().Name;
            var pathFile = string.Format("{0}.{1}.{2}", assemblyName, directoryName, fileNameOnly);
            
            return GetType().Assembly.GetManifestResourceStream(pathFile);
        }
    }
}
