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
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.Helpers;  

namespace Modulo.Collect.Probe.Windows
{
    public class StraightNetworkConnectionProvider : IConnectionProvider
    {
        private TargetInfo targetInfo { get; set; }

        public void Connect(TargetInfo target)
        {
            this.targetInfo = target;

            if (this.targetInfo.IsThereCredential())
                this.tryToConnectTarget();
        }

        private void tryToConnectTarget()
        {
            string remoteUNC = this.targetInfo.GetRemoteUNC();
            string fullUsername = this.targetInfo.credentials.GetFullyQualifiedUsername();
            string password = targetInfo.credentials.GetPassword();

            // The Win32 API doesnt support this kind of username. 
            // In order to connect in host without domain, 
            // it will be necessary replace the '.' character by target address.
            if (fullUsername.StartsWith(@".\"))
                fullUsername = 
                    string.Format(@"{0}\{1}", 
                        this.targetInfo.GetAddress(), 
                        this.targetInfo.credentials.GetUserName());
            try
            {
                WinNetUtils.connectToRemote(remoteUNC, fullUsername, password);
            }
            catch (Exception ex)
            {
                throw new Modulo.Collect.Probe.Common.Exceptions.ProbeException(ex.ToString());
            }
        }

        public void Disconnect()
        {
            try
            {
                WinNetUtils.disconnectRemote(targetInfo.GetRemoteUNC());
            }
            catch(Exception)
            {
            }
        }
    }
}
