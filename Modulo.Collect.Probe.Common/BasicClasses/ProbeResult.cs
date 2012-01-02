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
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Common.BasicClasses
{
    public class ProbeResult
    {
        public IEnumerable<CollectedObject> CollectedObjects { get; set; }
        public IEnumerable<ProbeLogItem> ExecutionLog { get; set; }
        public SystemInformation SystemInformation { get; set; }

        /// <summary>
        /// Determines whether this instance has errors.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </returns>
        public bool HasErrors()
        {
            int amountErrors = ExecutionLog.Where(e => e != null && e.Type == TypeItemLog.Error).Count();
            return amountErrors > 0;
        }

        public IEnumerable<ItemType> GetItemTypes()
        {
            List<ItemType> itemTypes = new List<ItemType>();
            foreach (CollectedObject collectedObject in CollectedObjects)
            {
                itemTypes.AddRange(collectedObject.SystemData);
            }
            return itemTypes;
        }
    }
}
