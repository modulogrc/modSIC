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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.Probes.Metabase;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.Metabase
{
    [TestClass]
    public class MetabaseSessionConfigurationTests
    {
        private const string NAME_SAMPLE = "MD_ISM_ACCESS_CHECK";
        private const string ID_SAMPLE = "6269";
        private const string VALUE_SAMPLE = "65535";
        private const string TYPE_SAMPLE = "DWORD";
        private const string USER_TYPE_SAMPLE = "IIS_MD_UT_FILE";

        /// <summary>
        /// Metabase Session Sample:
        /// 	<Custom
        /// 	    Name="MD_ISM_ACCESS_CHECK"
		/// 	    ID="6269"
		/// 	    Value="65535"
		/// 	    Type="DWORD"
		/// 	    UserType="IIS_MD_UT_FILE"
		/// 	    Attributes="NO_ATTRIBUTES"
        ///	    />
        /// </summary>
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_represent_metabase_session()
        {
            var metabaseSession = 
                new MetabaseSessionConfiguration(
                    NAME_SAMPLE, ID_SAMPLE, TYPE_SAMPLE, USER_TYPE_SAMPLE);

            Assert.AreEqual(NAME_SAMPLE, metabaseSession.Name, "Unexpected metabase name was found.");
            Assert.AreEqual(ID_SAMPLE, metabaseSession.ID, "Unexpected metabase ID was found.");
            Assert.AreEqual(TYPE_SAMPLE, metabaseSession.Type, "Unexpected metabase type was found.");
            Assert.AreEqual(USER_TYPE_SAMPLE, metabaseSession.UserType, "Unexpected metabase user type was found.");
        }

        [TestMethod, Owner("lfernandes")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_not_be_possible_to_create_a_metabase_session_with_null_ID()
        {
            new MetabaseSessionConfiguration(NAME_SAMPLE, null, TYPE_SAMPLE, USER_TYPE_SAMPLE);
            new MetabaseSessionConfiguration(NAME_SAMPLE, string.Empty, TYPE_SAMPLE, USER_TYPE_SAMPLE);
            new MetabaseSessionConfiguration(NAME_SAMPLE, "          ", TYPE_SAMPLE, USER_TYPE_SAMPLE);
        }

        [TestMethod, Owner("lfernandes")]
        public void All_metabase_properties_can_be_null_but_ID()
        {
            var metabaseSession = new MetabaseSessionConfiguration(null, ID_SAMPLE, null, null);
            
            Assert.IsNull(metabaseSession.Name, "Unexpected metabase name was found.");
            Assert.AreEqual(ID_SAMPLE, metabaseSession.ID, "Unexpected metabase ID was found.");
            Assert.IsNull(metabaseSession.Type, "Unexpected metabase type was found.");
            Assert.IsNull(metabaseSession.UserType, "Unexpected metabase user type was found.");
        }
    }
}
