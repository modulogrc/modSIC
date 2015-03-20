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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.Probes.Metabase;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.XmlFileContent;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.Metabase
{
    [TestClass]
    public class XmlDocumentNavigatorTests
    {
        #region consts metabase fragment samples
        private const string METABASE_FRAGMENT_SAMPLE =
            "<Custom Name='MD_ISM_ACCESS_CHECK' ID='6269' Value='65535' Type='DWORD' UserType='IIS_MD_UT_FILE' Attributes='NO_ATTRIBUTES' />" +
            "<Custom Name='UnknownName_2166' ID='2166' Value='1' Type='DWORD' UserType='IIS_MD_UT_SERVER' Attributes='NO_ATTRIBUTES' />" +
            "<Custom Name='UnknownName_9202' ID='9202' Value='4294967295' Type='DWORD' UserType='IIS_MD_UT_SERVER' Attributes='NO_ATTRIBUTES' />";

        private const string METABASE_FRAGMENT_MAX_CONNECTIONS_SAMPLE =
            "<IIsWebServer	Location ='/LM/W3SVC/1' " +
            "AppPoolId='DefaultAppPool' " +
            "DefaultDoc='Default.htm,Default.asp,index.htm,iisstart.htm' " +
            "LogExtFileFlags='LogExtFileDate | LogExtFileTime | LogExtFileClientIp | LogExtFileUserName | LogExtFileSiteName | LogExtFileServerIp | LogExtFileMethod | LogExtFileUriStem | LogExtFileUriQuery | LogExtFileHttpStatus | LogExtFileWin32Status | LogExtFileServerPort | LogExtFileUserAgent | LogExtFileHttpSubStatus' " +
            "LogFileLocaltimeRollover='FALSE' " +
            "LogFilePeriod='4' " +
            "LogFileTruncateSize='20971520' " +
            "MaxConnections='1001' " +
            "ServerBindings=':80:' " +
            "ServerComment='Default Web Site' " +
            "ServerSize='1' " +
            "SslCtlIdentifier='{E553F318-89D9-4C9F-9C09-91FF12A3BFAF}' " +
            "SslCtlStoreName='CA'></IIsWebServer>";

        private const string METABASE_FRAGMENT_WITH_FLAGGED_ATTRIBUTES_SAMPLE =
            " <IIsWebVirtualDir	Location =\"/LM/W3SVC/1/ROOT\" " +
            " AccessFlags=\"AccessRead | AccessWrite | AccessScript \"" +
            " AccessSSLFlags=\"AccessSSL\"" +
            " AppFriendlyName=\"Default Application\"" +
            " AppIsolated=\"2\"" +
            " AppPoolId=\"DefaultAppPool\"" +
            " AppRoot=\"/LM/W3SVC/1/ROOT\"" +
            " AuthFlags=\"AuthNTLM\"" +
            " Path=\"E:\\inetpub\\wwwroot\"" +
            " UNCPassword=\"\"" +
            " ></IIsWebVirtualDir>";

        private const string METABASE_FRAGMENT_WITH_FLAGGED_ATTRIBUTES_SAMPLE_2 =
            " <IIsWebService	Location =\"/LM/W3SVC\" " +
		    "     AllowKeepAlive=\"TRUE\"" +
		    "     InProcessIsapiApps=\"C:\\WINDOWS\\system32\\inetsrv\\httpext.dll \r\n " +
		    " 	    C:\\WINDOWS\\system32\\inetsrv\\httpodbc.dll \r\n " +
		    " 	    C:\\WINDOWS\\system32\\inetsrv\\ssinc.dll \r\n " +
		    " 	    C:\\WINDOWS\\system32\\msw3prt.dll\" \r\n " +
            "     DirBrowseFlags=\"DirBrowseShowDate | DirBrowseShowTime | DirBrowseShowSize | DirBrowseShowExtension | DirBrowseShowLongDate | EnableDefaultDoc\" " +
            "     LogExtFileFlags=\"LogExtFileDate | LogExtFileTime | LogExtFileClientIp | LogExtFileUserName | LogExtFileSiteName | LogExtFileServerIp | LogExtFileMethod | LogExtFileUriStem | LogExtFileUriQuery | LogExtFileHttpStatus | LogExtFileWin32Status | LogExtFileServerPort | LogExtFileUserAgent | LogExtFileHttpSubStatus\" " +
		    "     PasswordChangeFlags=\"AuthChangeDisable | AuthAdvNotifyDisable\" " +
            "     LogFileDirectory=\"C:\\WINDOWS\\system32\\LogFiles\" " +
	        " > " +
 	        "     <Custom " +
		    "         Name=\"UnknownName_9202\" " +
		    "         ID=\"9202\" " +
		    "         Value=\"4294967295\" " +
		    "         Type=\"DWORD\" " +
		    "         UserType=\"IIS_MD_UT_SERVER\" " +
		    "         Attributes=\"NO_ATTRIBUTES\" " +
	        "     /> " +
            " </IIsWebService>";


        private const string METABASE_FRAGMENT_WITH_FLAGGED_ATTRIBUTES_SAMPLE_3 =
            " <IIsWebService	Location =\"/LM/W3SVC\" " +
            "     AllowKeepAlive=\"TRUE\"" +
            "     AuthPersistence=\"\" " +
            "     FilterFlags=\"NotifySecurePort | NotifyAuthComplete\" " +
            "     ServerConfigFlags=\"ServerConfigSSL40 | ServerConfigSSL128 | ServerConfigSSLAllowEncrypt | ServerConfigAutoPWSync  \" " +
            "     AspAppServiceFlags=\"AspEnableTracker | AspEnableSxs\" " +
            "     LogEventOnRecycle=\"AppPoolRecycleTime | AppPoolRecycleMemory | AppPoolRecyclePrivateMemory \" " +
            "     > " +
            " </IIsWebService>";
        
        #endregion

        private String MBSchemaSample;

        public XmlDocumentNavigatorTests()
        {
            var allMbSchemaLines = 
                new FileContentLoader()
                    .ReadAllFileLinesFromResource("mbSchemaSample.xml");

            this.MBSchemaSample = string.Join(Environment.NewLine, allMbSchemaLines);

        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_find_metabase_fragment_by_id()
        {
            var metabaseFragment = 
                new MetabaseFragment(METABASE_FRAGMENT_MAX_CONNECTIONS_SAMPLE, MBSchemaSample);

            var metabaseSession = metabaseFragment.GetMetabaseSessionByID("1014");

            Assert.IsNotNull(metabaseSession);
            Assert.AreEqual("1014", metabaseSession.ID);
            Assert.AreEqual("MaxConnections", metabaseSession.Name);
            Assert.AreEqual("DWORD", metabaseSession.Type);
            Assert.AreEqual("IIS_MD_UT_SERVER", metabaseSession.UserType);
            Assert.AreEqual("1001", metabaseSession.Value);
        }

        [TestMethod, Owner("lfernandes")]
        [ExpectedException(typeof(XPathNoResultException))]
        public void When_metabase_property_does_not_exist_XPathNoResultException_must_be_throw()
        {
            new 
                MetabaseFragment(METABASE_FRAGMENT_MAX_CONNECTIONS_SAMPLE, MBSchemaSample)
                    .GetMetabaseSessionByID("1013");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_AccessSslFlags_by_id()
        {
            var metabaseFragment = 
                new MetabaseFragment(METABASE_FRAGMENT_WITH_FLAGGED_ATTRIBUTES_SAMPLE, MBSchemaSample);

            var metabaseSession = metabaseFragment.GetMetabaseSessionByID("6030");

            Assert.AreEqual("8", metabaseSession.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_AccessFlags_by_id()
        {
            var metabaseFragment = 
                new MetabaseFragment(METABASE_FRAGMENT_WITH_FLAGGED_ATTRIBUTES_SAMPLE, MBSchemaSample);

            var metabaseSession = metabaseFragment.GetMetabaseSessionByID("6016");

            Assert.AreEqual("515", metabaseSession.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_AuthFlags_by_id()
        {
            var metabaseFragment = 
                new MetabaseFragment(METABASE_FRAGMENT_WITH_FLAGGED_ATTRIBUTES_SAMPLE, MBSchemaSample);

            var metabaseSession = metabaseFragment.GetMetabaseSessionByID("6000");

            Assert.AreEqual("4", metabaseSession.Value);
        }
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_DirBrowseFlags_and_LogExtFileFlags_and_PasswordChangeFlags_by_id()
        {
            var metabaseFragment = 
                new MetabaseFragment(METABASE_FRAGMENT_WITH_FLAGGED_ATTRIBUTES_SAMPLE_2, MBSchemaSample);

            var dirBrowseFlags = metabaseFragment.GetMetabaseSessionByID("6005");
            var logExtFileFlags = metabaseFragment.GetMetabaseSessionByID("4013");
            var passwordChangeFlags = metabaseFragment.GetMetabaseSessionByID("2068");

            Assert.AreEqual("1073741886", dirBrowseFlags.Value);
            Assert.AreEqual("2199519", logExtFileFlags.Value);
            Assert.AreEqual("6", passwordChangeFlags.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_the_others_flagged_properties_by_id()
        {
            var metabaseFragment = 
                new MetabaseFragment(METABASE_FRAGMENT_WITH_FLAGGED_ATTRIBUTES_SAMPLE_3, MBSchemaSample);

            var authPersistence = metabaseFragment.GetMetabaseSessionByID("6031");
            var filterFlags = metabaseFragment.GetMetabaseSessionByID("2044");
            var serverConfigFlags = metabaseFragment.GetMetabaseSessionByID("1027");
            var aspAppServiceFlags = metabaseFragment.GetMetabaseSessionByID("7044");
            var logEventOnRecycle = metabaseFragment.GetMetabaseSessionByID("9037");

            Assert.AreEqual("", authPersistence.Value);
            Assert.AreEqual("67108865", filterFlags.Value);
            Assert.AreEqual("15", serverConfigFlags.Value);
            Assert.AreEqual("3", aspAppServiceFlags.Value);
            Assert.AreEqual("137", logEventOnRecycle.Value);
        }
    }
}
