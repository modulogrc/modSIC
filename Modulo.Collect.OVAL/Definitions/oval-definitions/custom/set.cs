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

namespace Modulo.Collect.OVAL.Definitions
{
    public partial class set
    {

        /// <summary>
        /// Gets the item value from the items array.
        /// This method makes the combination between ItemsElementName and Items array
        /// to the get value.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns></returns>
        public Object GetItemValue(set_items itemName)
        {
            foreach (var item in this.Items)
            {
                switch (itemName)
                {
                    case set_items.filter:
                        if (item is filter)
                            return ((filter)item).Value;
                        break;
                    case set_items.set:
                        if (item is set)
                            return item;
                        break;
                    default:break;
                }
            }
             
            return null;
        }

        /// <summary>
        /// This method verifies if exists another Set element configured in the current set.
        /// </summary>
        /// <returns></returns>
        public bool ExistsAnotherSetElement()
        {
            Object setElement = this.GetItemValue(set_items.set);
            if (setElement != null)
                return true;

            return false;
        }

        /// <summary>
        /// Get the list of references for the objects in the Set.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetObjectReferences()
        {
            List<string> references = new List<string>();
            foreach (var item in this.Items)
            {
                if (item is string)
                    references.Add((string)item);
            }
            return references;
        }


        /// <summary>
        /// This method return all the sets that defined in the a SetObject.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<set> GetSets()
        {
            List<set> sets = new List<set>();
            foreach (var item in this.Items)
            {
                if (item is set)
                    sets.Add((set)item);
            }
            return sets;
            
        }
        /// <summary>
        /// This method verifies if exists a Set element configured in the current set.
        /// </summary>        
        public bool HasFilterElement()
        {
            Object setElement = this.GetItemValue(set_items.filter);
            if (setElement != null)
                return true;
            return false;
        }

        /// <summary>
        /// Gets the filter value.
        /// </summary>
        /// <returns></returns>
        public string GetFilterValue()
        {
            if (!this.HasFilterElement())
                return "";

            Object element = this.GetItemValue(set_items.filter);
            return element.ToString();
            
        }
    }
    
    public enum set_items
    {
        set,
        filter,
    }

}
