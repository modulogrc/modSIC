#region License
/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */
#endregion

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Modulo.Collect.ClientConsole.Tests
{
    [TestClass]
    public class CheckParametersTest
    {
        [TestMethod, Owner("cpaiva"), ExpectedException(typeof(System.ArgumentNullException))]
        public void Shouldnt_be_possible_get_an_operation_code_from_a_null_command_line()
        {
            MockRepository mocks = new MockRepository();
            var fakeView = mocks.DynamicMock<IMainView>();
            var fakeModel = mocks.DynamicMock<IMainModel>();

            Expect.Call(delegate { fakeView.ShowMessage(null); }).IgnoreArguments();

            var fakePresenter = new MainPresenter(fakeView, fakeModel);
            fakePresenter.PrepareOptions(null);
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());
        }

        [TestMethod, Owner("cpaiva")]
        public void Shouldnt_be_possible_get_an_operation_code_from_a_invalid_command_line()
        {
            MockRepository mocks = new MockRepository();
            var fakeView = mocks.DynamicMock<IMainView>();
            var fakeModel = mocks.DynamicMock<IMainModel>();
            var fakePresenter = mocks.DynamicMock<MainPresenter>(new object[] {fakeView, fakeModel});

            ServerSection fakeServerSection = new ServerSection();
            fakeServerSection.Server.address = "127.0.0.1";
            fakeServerSection.Server.username = "admin";
            fakeServerSection.Server.password = "87654321";
            fakeServerSection.Collects.Add(new CollectElement() { Name="test", Target="10.0.0.1", Username="user", Password="12345678", Definitions="test-definitions.xml" });           
            Expect.Call(fakePresenter.ReadServerSection()).IgnoreArguments().Return(fakeServerSection);

            mocks.ReplayAll();

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] {""}));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-s" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-g", "-x" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-l" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-s" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-g" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-e" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-e", "test" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());
            
            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-g", "collectrequests/1" }));
            Assert.AreEqual(Operation.GetResults, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-x" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-x", "collectrequests/1" }));
            Assert.AreEqual(Operation.CancelCollect, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-l" }));
            Assert.AreEqual(Operation.ListCollectsInExecution, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-n" }));
            Assert.AreEqual(Operation.ValidateSchematronOnly, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-n", "-e", "test" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--collect"}));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--collect-sync" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "--get-results" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--get-results", "collectrequests/1" }));
            Assert.AreEqual(Operation.GetResults, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--get-results", "collectrequests/1", "-e", "test" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "--cancel" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--cancel", "collectrequests/1" }));
            Assert.AreEqual(Operation.CancelCollect, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--cancel", "collectrequests/1", "-e", "test" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--list" }));
            Assert.AreEqual(Operation.ListCollectsInExecution, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--list", "-e", "tests" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--list", "--preset", "test" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            //  Send Collect
            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.SendCollect, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-n", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.SendCollect, fakePresenter.CheckParameters());
            
            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-c", "-n", "-e", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-n", "-e", "test", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.SendCollect, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-n", "-e", "test", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.SendCollect, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-n", "-e", "test" }));
            Assert.AreEqual(Operation.SendCollect, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-n", "-e", "default", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());           
            
            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-c", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.SendCollect, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--collect", "--modsic", "http://localhost:1024", "--username", "admin", "--password", "Pa$$w@rd", "--target", "10.0.0.1", "--target-username", "administrator", "--target-password", "P@$$w@rd" }));
            Assert.AreEqual(Operation.SendCollect, fakePresenter.CheckParameters());

            //  Send Collect Synchronous
            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-s", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-s", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.SendCollectSync, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-s", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.SendCollectSync, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--collect-sync", "--modsic", "http://localhost:1024", "--username", "admin", "--password", "Pa$$w@rd", "--target", "10.0.0.1", "--target-username", "administrator", "--target-password", "P@$$w@rd" }));
            Assert.AreEqual(Operation.SendCollectSync, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-s", "-n", "-e", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-s", "-n", "-e", "test", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.SendCollectSync, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-s", "-n", "-e", "test", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.SendCollectSync, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-s", "-n", "-e", "test" }));
            Assert.AreEqual(Operation.SendCollectSync, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-s", "-n", "-e", "default", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            //  Get Results
            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-g", "collectrequests/1", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.GetResults, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-g", "-n", "collectrequests/1", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-g", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-g", "collectrequests/1", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-g", "collectrequests/1", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--get-results", "collectrequests/1", "--modsic", "http://localhost:1024", "--username", "admin", "--password", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.GetResults, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--get-results", "collectriquestis/1", "--modsic", "http://localhost:1024", "--username", "admin", "--password", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            //  Cancel Collect
            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-x", "collectrequests/1", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.CancelCollect, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-x", "-n", "collectrequests/1", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-x", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-x", "collectrequests/1", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-x", "collectrequests/1", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--cancel", "collectrequests/1", "--modsic", "http://localhost:1024", "--username", "admin", "--password", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.CancelCollect, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--cancel", "collectriquestis/1", "--modsic", "http://localhost:1024", "--username", "admin", "--password", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            //  List collects in execution
            Assert.IsFalse(fakePresenter.PrepareOptions(new String[] { "-l", "collectrequests/1", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-l", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.ListCollectsInExecution, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-l", "-n", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-l", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "-l", "-m", "http://localhost:1024", "-u", "admin", "-p", "Pa$$w@rd", "-t", "10.0.0.1", "-y", "administrator", "-z", "P@$$w@rd" }));
            Assert.AreEqual(Operation.Error, fakePresenter.CheckParameters());

            Assert.IsTrue(fakePresenter.PrepareOptions(new String[] { "--list", "--modsic", "http://localhost:1024", "--username", "admin", "--password", "Pa$$w@rd" }));
            Assert.AreEqual(Operation.ListCollectsInExecution, fakePresenter.CheckParameters());

            mocks.VerifyAll();
        }   
    }
}
