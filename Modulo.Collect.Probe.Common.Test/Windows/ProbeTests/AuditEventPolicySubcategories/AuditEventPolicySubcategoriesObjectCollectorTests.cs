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
using Modulo.Collect.Probe.Windows.AuditEventPolicySubcategories;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.AuditEventPolicy;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Windows.Test.AuditEventPolicySubcategories
{
    [TestClass]
    public class AuditEventPolicySubcategoriesObjectCollectorTests
    {
        private const string UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG = "An unexpected audit event subcategory policy was found.";
        
        private static string AUDIT_FAILURE;
        private static string AUDIT_NONE;
        private static string AUDIT_SUCCESS;
        private static string AUDIT_SUCCESS_FAILURE;
        private TargetInfo FakeTargetInfo;


        public AuditEventPolicySubcategoriesObjectCollectorTests()
        {
            AUDIT_FAILURE = AuditEventStatus.AUDIT_FAILURE.ToString();
            AUDIT_NONE = AuditEventStatus.AUDIT_NONE.ToString();
            AUDIT_SUCCESS = AuditEventStatus.AUDIT_SUCCESS.ToString();
            AUDIT_SUCCESS_FAILURE = AuditEventStatus.AUDIT_SUCCESS_FAILURE.ToString();
            this.FakeTargetInfo = ProbeHelper.CreateFakeTarget();
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_collect_a_AuditEventPolicySubcategoriesItem()
        {
            AuditEventPolicyHelper fakeHelper;
            var fakeItemType = AuditEventPolicySubcategoriesItemTypeGenerator.CreateAuditEventPolicySubcategoriesItemType();
            var allFakeSubcategories = this.GetFakeAuditEventSubcategories();
            var systemDataSource = this.CreateMockedSystemDataSource(allFakeSubcategories, out fakeHelper);


            var result = systemDataSource.CollectDataForSystemItem(fakeItemType);
            
            
            Assert.IsNotNull(result, "The result of system datasource collection");
            Assert.AreEqual(1, result.Count(), "Unexpected items count was found.");
            var itemToAssert = (auditeventpolicysubcategories_item)result.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.exists, itemToAssert.status, "An error occurred while trying to collect item type.");

            Assert.AreEqual(AUDIT_FAILURE, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.account_lockout), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);
            Assert.AreEqual(AUDIT_NONE, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.computer_account_management), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);
            Assert.AreEqual(AUDIT_SUCCESS, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.directory_service_changes), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);
            Assert.AreEqual(AUDIT_SUCCESS_FAILURE, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.file_system), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);

            Assert.AreEqual(AUDIT_FAILURE, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.handle_manipulation), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);
            Assert.AreEqual(AUDIT_NONE, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.ipsec_driver), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);
            Assert.AreEqual(AUDIT_SUCCESS_FAILURE, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.mpssvc_rule_level_policy_change), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);

            Assert.AreEqual(AUDIT_FAILURE, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.other_object_access_events), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);
            Assert.AreEqual(AUDIT_NONE, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.process_creation), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);
            Assert.AreEqual(AUDIT_SUCCESS, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.registry), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);
            Assert.AreEqual(AUDIT_SUCCESS_FAILURE, this.GetSubcategoryValue(itemToAssert, AuditEventSubcategories.security_state_change), UNEXPECTED_SUBCATEGORY_VALUE_ASSERT_FAILED_MSG);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_return_collected_items_from_an_item_with_error_status()
        {
            AuditEventPolicyHelper fakeHelper;
            var systemDataSource = this.CreateMockedSystemDataSource(new Dictionary<AuditEventSubcategories, AuditEventStatus>(), out fakeHelper);

            systemDataSource.CollectDataForSystemItem(new auditeventpolicysubcategories_item() { status = StatusEnumeration.error });

            fakeHelper.AssertWasNotCalled<AuditEventPolicyHelper>(f => f.GetAuditEventSubcategoriesPolicy(FakeTargetInfo));
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_generate_items_with_error_status_when_an_exception_was_thrown()
        {
            MockRepository mocks = new MockRepository();
            var fakeHelper = mocks.DynamicMock<AuditEventPolicyHelper>(this.FakeTargetInfo);
            Expect.Call(fakeHelper.GetAuditEventSubcategoriesPolicy(null)).IgnoreArguments().Throw(new Exception("Test Execption was thrown"));
            mocks.ReplayAll();
            var systemDataSource = new AuditEventPolicySubcategoriesObjectCollector() { AuditEventPolicyHelper = fakeHelper };


            var collectedItem = systemDataSource.CollectDataForSystemItem(new auditeventpolicysubcategories_item()).First().ItemType;


            Assert.AreEqual(StatusEnumeration.error, collectedItem.status);
            Assert.IsNotNull(collectedItem.message);
            Assert.IsTrue(collectedItem.message.First().Value.Contains("AuditEventPolicySubcategoriesObjectCollector"));
        }

        


        private Dictionary<AuditEventSubcategories, AuditEventStatus> GetFakeAuditEventSubcategories()
        {
            var result = new Dictionary<AuditEventSubcategories, AuditEventStatus>();
            
            result.Add(AuditEventSubcategories.account_lockout, AuditEventStatus.AUDIT_FAILURE);
            result.Add(AuditEventSubcategories.computer_account_management, AuditEventStatus.AUDIT_NONE);
            result.Add(AuditEventSubcategories.directory_service_changes, AuditEventStatus.AUDIT_SUCCESS);
            result.Add(AuditEventSubcategories.file_system, AuditEventStatus.AUDIT_SUCCESS_FAILURE);

            result.Add(AuditEventSubcategories.handle_manipulation, AuditEventStatus.AUDIT_FAILURE);
            result.Add(AuditEventSubcategories.ipsec_driver, AuditEventStatus.AUDIT_NONE);
            result.Add(AuditEventSubcategories.mpssvc_rule_level_policy_change, AuditEventStatus.AUDIT_SUCCESS_FAILURE);

            result.Add(AuditEventSubcategories.other_object_access_events, AuditEventStatus.AUDIT_FAILURE);
            result.Add(AuditEventSubcategories.process_creation, AuditEventStatus.AUDIT_NONE);
            result.Add(AuditEventSubcategories.registry, AuditEventStatus.AUDIT_SUCCESS);
            result.Add(AuditEventSubcategories.security_state_change, AuditEventStatus.AUDIT_SUCCESS_FAILURE);
            
            return result;
        }

        private AuditEventPolicySubcategoriesObjectCollector CreateMockedSystemDataSource(
            Dictionary<AuditEventSubcategories, AuditEventStatus> fakeAuditEventSubcategoriesToReturn, out AuditEventPolicyHelper fakeHelper)
        {
            MockRepository mocks = new MockRepository();
            fakeHelper = mocks.DynamicMock<AuditEventPolicyHelper>(this.FakeTargetInfo);
            Expect.Call(fakeHelper.GetAuditEventSubcategoriesPolicy(null)).IgnoreArguments().Return(fakeAuditEventSubcategoriesToReturn);
            mocks.ReplayAll();

            return new AuditEventPolicySubcategoriesObjectCollector() { AuditEventPolicyHelper = fakeHelper };
        }

        private string GetSubcategoryValue(auditeventpolicysubcategories_item itemType, AuditEventSubcategories subcategoryName)
        {
            var subCategoryField = itemType.GetType().GetProperty(subcategoryName.ToString());
            var itemValueField = subCategoryField.GetValue(itemType, null);
            var itemValueFieldValue = itemValueField.GetType().GetProperty("Value");
            return itemValueFieldValue.GetValue(itemValueField, null).ToString();
        }


    }
}
