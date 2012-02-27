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
using Modulo.Collect.Probe.Common;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace Modulo.Collect.Probe.Unix
{
    public class SSHConnectionProvider: IConnectionProvider
    {
        public SshExec SSHExec { get; private set; }
        
        public virtual void Connect(TargetInfo target)
        {
            this.CreateSSHExec(target);
            try
            {
                this.SSHExec.Connect(target.GetPort());
            }
            catch (JSchException ex)
            {
                throw new SshConnectingException(
                    string.Format(
                        "Unable to connect to target machine {0} through port {1} using the user {2}. Check the target address (or host name), port number and that ssh service is running at target machine.",
                        target.GetAddress(), target.GetPort(), target.credentials.GetFullyQualifiedUsername()));
            }
        }

        public virtual void Disconnect()
        {
            try
            {
                this.SSHExec.Close();
            }
            catch (Exception)
            {
            }
        }

        public virtual Boolean IsTargetAUnixSystem(TargetInfo target)
        {
            try
            {
                this.Connect(target);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                this.Disconnect();
            }
        }

        private void CreateSSHExec(TargetInfo target)
        {
            var credentials = target.credentials;
            this.SSHExec = new SshExec(target.GetAddress(), credentials.GetUserName(), credentials.GetPassword());
            
        }

    }

    public class SshConnectingException : Exception
    {
        public SshConnectingException(string message) : base(message) { }
    }
}
