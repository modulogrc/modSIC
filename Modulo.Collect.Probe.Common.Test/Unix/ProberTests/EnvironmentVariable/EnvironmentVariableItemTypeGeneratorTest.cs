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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Unix.EnvironmentVariable;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Test.Checkers;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.EnvironmentVariable
{
    [TestClass]
    public class EnvironmentVariableItemTypeGeneratorTest
    {
         [TestMethod, Owner("cpaiva")]
         public void Should_be_to_generate_environmentvariable_items_from_an_simple_object()
         {
             var fakeOvalObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "2360");
             var itemsToCollect = new EnvironmentVariableItemTypeGenerator().GetItemsToCollect(fakeOvalObject, null).ToArray();

             ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(environmentvariable_item));
             Assert.AreEqual("PATH", ((environmentvariable_item)itemsToCollect.Single()).name.Value);
         }

         [TestMethod, Owner("cpaiva")]
         public void Should_be_possible_to_generate_environmentvariable_items_fron_an_object_with_referenced_variable()
         {
             var fakeOvalObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "2370");
             var fakeVariables = VariableHelper.CreateVariableWithOneValue("2370", "2370", "CWD");

             var itemsToCollect = new EnvironmentVariableItemTypeGenerator().GetItemsToCollect(fakeOvalObject, fakeVariables).ToArray();

             ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(environmentvariable_item));
             Assert.AreEqual("CWD", ((environmentvariable_item)itemsToCollect.Single()).name.Value);

         }


    }
}
