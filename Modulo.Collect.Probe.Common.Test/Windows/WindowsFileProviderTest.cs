using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.File;

namespace Modulo.Collect.Probe.Common.Test
{


    [TestClass]
    public class WindowsFileProviderTest
    {
        [TestMethod, Owner("lfernandes")]
        public void CreateWmiParametersTest()
        {
            var target = new WindowsFileProvider(null);
            
            var fileParameters = target.CreateWmiParameters("c:\\temp\\file.ext");
            CheckWmiParametersCount(fileParameters, 4);
            AssertWmiFileFilter(fileParameters, WqlFileParameters.Drive, "c:");
            AssertWmiFileFilter(fileParameters, WqlFileParameters.Path, @"\\temp\\");
            AssertWmiFileFilter(fileParameters, WqlFileParameters.FileName, "file");
            AssertWmiFileFilter(fileParameters, WqlFileParameters.Extension, "ext");

            var folderParameters = target.CreateWmiParameters("c:\\temp\\sgct");
            CheckWmiParametersCount(folderParameters, 3);
            Assert.IsFalse(folderParameters.ContainsKey(WqlFileParameters.Extension.ToString()), "For folder searching extension parameter should not exists");
            AssertWmiFileFilter(folderParameters, WqlFileParameters.Drive, "c:");
            AssertWmiFileFilter(folderParameters, WqlFileParameters.Path, @"\\temp\\");
            AssertWmiFileFilter(folderParameters, WqlFileParameters.FileName, "sgct");
        }


        private void CheckWmiParametersCount(Dictionary<string, string> result, int expectedCount)
        {
            Assert.IsNotNull(result, "Result cannot be null.");
            Assert.AreEqual(expectedCount, result.Count, "Unexpected amount of wmi parameters.");
        }

        private void AssertWmiFileFilter(Dictionary<string, string> result, WqlFileParameters wmiParameterName, string expectedValue)
        {
            string parameter = null;
            result.TryGetValue(wmiParameterName.ToString(), out parameter);
            if (parameter == null)
                Assert.Fail("Expected wmi parameter was not found: '{0}'", wmiParameterName.ToString());

            Assert.AreEqual(expectedValue, parameter);
        }
    }
}
