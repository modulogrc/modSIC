using Modulo.Collect.Service.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Modulo.Collect.Service.Client.Internal;
using Modulo.Collect.Service.Contract;
using Rhino.Mocks;

namespace Modulo.Collect.Service.API.Tests
{
    [TestClass]
    public class ModSicApiExceptionTests
    {
        [TestMethod, Owner("lfernandes")]
        public void ModsicCallingExceptionTest()
        {
            var callingException = new ModSicCallingException("Fake Exception Message.");

            Assert.AreEqual("Fake Exception Message.", callingException.Message);
        }
    }
}
