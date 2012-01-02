using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.IO;
using System.Reflection;

namespace Modulo.Collect.GraphicalConsole.Tests
{
    [TestClass]
    public class CollectionControllerTest
    {
        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_raising_a_collection_event_on_a_view()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = mocks.DynamicMock<ICollectionView>();

            mocks.ReplayAll();

            var controller = new CollectionController(fakeView);

            var e = new RequestCollectionEvenArgs();

            fakeView.Raise(x => x.OnRequestCollection += null, this, e);

            mocks.VerifyAll();

            Assert.IsTrue(controller.OnRequestCollectionCalled);
        }
    }
}
