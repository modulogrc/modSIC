/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
using Modulo.Collect.Probe.Windows.WMI;
using System.Management;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Windows
{
    public class WmiDataProviderFactory
    {
        private static string CIMV2_NAMESPACE = @"\\{0}\root\cimv2";

        public static WmiDataProvider CreateWmiDataProviderForFileSearching(string targetAddress)
        {
            var wmiNamespace = getNamespaceForTargetAddress(targetAddress);
            return new WmiDataProvider(wmiNamespace);
        }

        public static WmiDataProvider CreateWmiDataProviderForFileSearching(TargetInfo targetInfo)
        {
            var wmiNamespace = getNamespaceForTargetAddress(targetInfo.GetAddress());

            var username = string.Empty;
            var password = string.Empty;
            if (targetInfo.IsThereCredential())
            {
                username = targetInfo.credentials.GetFullyQualifiedUsername();
                password = targetInfo.credentials.GetPassword();
            }
            
            var connectionOptions = CreateConnectionOptions(username, password);

            var wmiConnectionScope = new ManagementScope(wmiNamespace, connectionOptions);
            wmiConnectionScope.Connect();

            return new WmiDataProvider(wmiConnectionScope);
        }

        public static ConnectionOptions CreateConnectionOptions(string username, string password)
        {
            var connectionOptions = GetConnectionOptions();
            if (!string.IsNullOrWhiteSpace(username))
            {
                connectionOptions.Username = username;
                connectionOptions.Password = password;
            }

            return connectionOptions;
        }

        public static ConnectionOptions CreateConnectionOptions(TargetInfo target)
        {
            var connectionOptions = GetConnectionOptions();
            if (!target.IsLocalTarget())
            {
                connectionOptions.Username = target.credentials.GetFullyQualifiedUsername();
                connectionOptions.Password = target.credentials.GetPassword();
            }

            return connectionOptions;
        }

        private static String getNamespaceForTargetAddress(string targetAddress)
        {
            return string.Format(CIMV2_NAMESPACE, targetAddress);
        }

        private static ConnectionOptions GetConnectionOptions()
        {
            return new ConnectionOptions()
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.Default,
                EnablePrivileges = true
            };
        }
    }
}
