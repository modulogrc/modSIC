using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Independent.XmlFileContent;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Unix.TextFileContent54;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Common.Test.Independent
{
    [TestClass]
    public class XmlFileContentItemTypeGeneratorTestes
    {
        [TestMethod]
        public void Should_be_possible_to_create_xmlfilecontent_items_for_unix_platform()
        {
            var mocks = new MockRepository();
            var pathOperator = mocks.DynamicMock<PathOperatorEvaluator>(new object[] { null, FamilyEnumeration.unix });
            pathOperator.FileProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(pathOperator.FileProvider.FileExists(null)).IgnoreArguments().Return(true);
            Expect.Call(pathOperator.ProcessOperationsPaths(null)).IgnoreArguments().Return(new[] { "/etc/webapps" });
            Expect.Call(pathOperator.ProcessOperationFileName(null, null, false)).IgnoreArguments().Return(new[] { "server.xml" });
            mocks.ReplayAll();
            var sampleObject = new LoadOvalDocument().GetFakeOvalDefinitions("definitions_all_unix.xml").objects.Single(obj => obj.id == "oval:modulo:obj:333");
            var itemtypeGenerator = new XmlFileContentItemTypeGenerator(null) { PathOperatorEvaluator = pathOperator };

            var itemsToCollect = itemtypeGenerator.GetItemsToCollect(sampleObject, null);

            var xmlFileContentItem = (xmlfilecontent_item)itemsToCollect.Single();
            Assert.AreEqual("/etc/webapps/server.xml", xmlFileContentItem.filepath.Value);

        }

    }
}
