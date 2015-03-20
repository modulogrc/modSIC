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

namespace Modulo.Collect.Probe.Unix.SSHCollectors
{
    public class LinuxPackageInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Arch { get; set; }
        public string Release { get; set; }
        public uint Epoch { get; set; }
        public string Signature { get; set; }

        public string Revision
        {
            get
            {
                return this.Release;
            }
            set
            {
                this.Release = value;
            }
        }

        public string Evr
        {
            get
            {
                if (this.Epoch > 0)
                    return String.Format("{0}:{1}-{2}", this.Epoch, this.Version, this.Release);
                else
                    return String.Format("{0}-{1}", this.Version, this.Release);
            }
            set
            {
                string verAndRelease;

                int whereEpoch = value.IndexOf(':');
                if (whereEpoch >= 0)
                {
                    this.Epoch = uint.Parse(value.Substring(0, whereEpoch));
                    verAndRelease = value.Substring(whereEpoch + 1);
                }
                else
                {
                    this.Epoch = 0;
                    verAndRelease = value;
                }

                int whereRel = verAndRelease.IndexOf('-');
                if (whereRel >= 0)
                {
                    this.Version = verAndRelease.Substring(0, whereRel);
                    this.Release = verAndRelease.Substring(whereRel + 1);
                }
                else
                {
                    this.Version = verAndRelease;
                    this.Release = "0";
                }
            }
        }

        public LinuxPackageInfo()
        {
            this.Epoch = 0;
        }

        public override string ToString()
        {
            string stringize = String.Format("Name <{0}> Version <{1}> Arch <{2}> Release <{3}>", Name, Version, Arch, Release);
            if (Epoch > 0)
                stringize += String.Format(" Epoch <{0}>", Epoch);
            return stringize;
        }
    }
}
