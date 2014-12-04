/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.UserSID55;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.UserSID55
{
    [TestClass]
    public class WindowsUserAccountBuilderTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_an_EnabledWindowsUserAccount_by_builder()
        {
            var winUserAccount =
                new WindowsUserAccountBuilder()
                    .WithSID("S-1-15")
                    .WithTrusteeName("mss\\lfernandes")
                    .WithGroups(new string[] { "S-1-20", "S-1-25", "S-1-30" })
                .Build();

            Assert.AreEqual(
                "S-1-15", 
                winUserAccount.UserSID, 
                UnexpectedPropertyInBuiltObject("UserSID"));

            Assert.AreEqual(
                "mss\\lfernandes", 
                winUserAccount.UserTrusteeName, 
                UnexpectedPropertyInBuiltObject("UserTrusteeName"));

            Assert.AreEqual(
                "S-1-20", 
                winUserAccount.GroupSIDs.ElementAt(0), 
                UnexpectedPropertyInBuiltObject("GroupSID[0]"));
            
            Assert.AreEqual(
                "S-1-25", 
                winUserAccount.GroupSIDs.ElementAt(1), 
                UnexpectedPropertyInBuiltObject("GroupSID[1]"));
            
            Assert.AreEqual(
                "S-1-30", 
                winUserAccount.GroupSIDs.ElementAt(2), 
                UnexpectedPropertyInBuiltObject("GroupSID[2]"));
            
            Assert.IsTrue(
                winUserAccount.UserEnabled, 
                UnexpectedPropertyInBuiltObject("UserEnabled"));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_an_DisabledWindowsUserAccountWithNoGroups_by_builder()
        {
            var winUserAccount =
                new WindowsUserAccountBuilder()
                    .WithSID("S-1-15")
                    .WithTrusteeName("microsoft\\bgates")
                    .Disabled()
                .Build();

            Assert.AreEqual(
                "S-1-15", 
                winUserAccount.UserSID, 
                UnexpectedPropertyInBuiltObject("UserSID"));

            Assert.AreEqual(
                "microsoft\\bgates", 
                winUserAccount.UserTrusteeName, 
                UnexpectedPropertyInBuiltObject("UserTrusteeName"));

            Assert.IsNull(
                winUserAccount.GroupSIDs, 
                UnexpectedPropertyInBuiltObject("GroupSID"));

            Assert.IsFalse(
                winUserAccount.UserEnabled, 
                UnexpectedPropertyInBuiltObject("UserEnabled"));
        }

        private string UnexpectedPropertyInBuiltObject(string propertyName)
        {
            return string.Format("Unexpected '{0}' was found on built windows user account.", propertyName);
        }

    }
}
