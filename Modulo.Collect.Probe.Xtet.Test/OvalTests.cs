using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.SapCode;
using Modulo.Collect.OVAL.Plugins;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics.SapCode;
using System.Linq;
using Modulo.Collect.Probe.CodeControl;

namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass]
    public class OvalTests
    {

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_read_sapcode_objects()
        {
            PluginContainer.RegisterOvalAssembly(typeof(sapcode_object).Assembly);
            var def = GetFakeOvalDefinitions("definitions_all_sapcode.xml");

            Assert.AreEqual(26, def.objects.OfType<sapcode_object>().Count());
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
