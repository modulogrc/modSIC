using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Unix.Probes.Process;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.Process
{
    [TestClass]
    public class ProcessProberTests: ProberTestBase
    {
        private  process_item FakeProcessItem;
        private  CollectedItem FakeCollectedItem;

        public ProcessProberTests()
        {
            this.FakeProcessItem = new process_item();
            this.FakeCollectedItem = ProbeHelper.CreateFakeCollectedItem(new OVAL.SystemCharacteristics.Unix.process_item());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_process_object()
        {         
            var processProber = new ProcessProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    processProber,
                    new ItemType[] { FakeProcessItem },
                    new CollectedItem[] { FakeCollectedItem });

            var probeResult =
                processProber
                    .Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("6", "definitions_all_unix.xml"));

            DoAssertForSingleCollectedObject(probeResult, typeof(OVAL.SystemCharacteristics.Unix.process_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_exceptions_while_trying_to_collect_process_object()
        {
            var processProber = new ProcessProber();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(processProber);

            var probeResult =
                processProber
                    .Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("6", "definitions_all_unix.xml"));
            
            DoAssertForExecutionWithErrors(probeResult, typeof(OVAL.SystemCharacteristics.Unix.process_item));
        }        

    
    }
}
