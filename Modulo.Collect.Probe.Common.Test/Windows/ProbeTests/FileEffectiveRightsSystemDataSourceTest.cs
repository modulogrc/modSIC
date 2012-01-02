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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using System;

namespace Modulo.Collect.Probe.Windows.Test
{
    public class MyObject
    {
        public Dictionary<string, object> Properties { get; set; }

        public void AddProperty(string name, object Value)
        {
            if (this.Properties == null)
                this.Properties = new Dictionary<string, object>();
            
            this.Properties.Add(name, Value);
        }
        
    }


    [TestClass]
    public class FileEffectiveRightsSystemDataSourceTest
    {
        [Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_collect_FileEffectiveRights()
        {
            // Arrange
            WmiObject fakeWmiObject = new WmiObject();
            fakeWmiObject.Add("Descriptor.DACL.AccessMask", (uint)128);
            fakeWmiObject.Add("Descriptor.DACL.AceFlags", (uint)123);
            fakeWmiObject.Add("Descriptor.DACL.Trustee.SID", "{500}");
            fakeWmiObject.Add("Descriptor.DACL.Trustee.Domain", "mss");
            fakeWmiObject.Add("Descriptor.DACL.Trustee.Name", "lfernandes");
            

            MockRepository mocks = new MockRepository();
            WmiDataProvider fakeWmiDataProvider = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(fakeWmiDataProvider.InvokeMethodByWmiPath(null)).IgnoreArguments().Return(new WmiObject[] { fakeWmiObject });
            FileEffectiveRightsObjectCollector fileEffectiveRightsSysDataSource = new FileEffectiveRightsObjectCollector();
            fileEffectiveRightsSysDataSource.WmiDataProvider = fakeWmiDataProvider;

            mocks.ReplayAll();

            // Act
            IEnumerable<CollectedItem> collectedItem = fileEffectiveRightsSysDataSource.CollectDataForSystemItem(this.getFakeFileItem());

            Assert.IsNotNull(collectedItem, "The return of collect data cannot be null");
            Assert.AreEqual(1, collectedItem.Count(), "Unexpected collected items count.");
            Assert.IsNotNull(collectedItem.ElementAt(0).ItemType, "The file item cannot be null.");
            Assert.IsInstanceOfType(collectedItem.ElementAt(0).ItemType, typeof(fileeffectiverights_item), "The item of collected item must be file_item");
            fileeffectiverights_item collectedFileEffectiveRights = (fileeffectiverights_item)collectedItem.ElementAt(0).ItemType;
            Assert.AreEqual("{500}", collectedFileEffectiveRights.trustee_sid.Value, "Unexpected item value.");
            Assert.AreEqual("1", collectedFileEffectiveRights.file_read_attributes.Value, "Unexpected file attribute found.");

            this.AssertFileEffectiveRight(collectedFileEffectiveRights.file_append_data, false, "file_append_data");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.file_delete_child, false, "file_delete_child");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.file_execute, false, "file_execute");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.file_read_data, false, "file_read_data");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.file_read_ea, false, "file_read_ea");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.file_write_attributes, false, "file_write_attributes");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.file_write_data, false, "file_write_data");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.file_write_ea, false, "file_write_ea");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.generic_all, true, "generic_all");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.generic_execute, true, "generic_execute");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.generic_read, true, "generic_read");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.generic_write, false, "generic_write");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.standard_delete, false, "standard_delete");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.standard_read_control, false, "standard_delete");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.standard_synchronize, false, "standard_sync");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.standard_write_dac, false, "standard_write_dac");
            this.AssertFileEffectiveRight(collectedFileEffectiveRights.standard_write_owner, false, "standard_write_owner");
        }

        private void AssertFileEffectiveRight(EntityItemBoolType userRightItem, bool hasRight, string rightDescription)
        {
            string convertedHasRight = hasRight ? "1" : "0";
            Assert.AreEqual(convertedHasRight, userRightItem.Value, string.Format("Unexpected file effective right '{0}' found for user", rightDescription));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_DACL_object_for_a_user()
        {
            string[] parentObjects = new string[] { "Descriptor", "DACL" };
            string[] propertyName = new string[] { "AccessMask", "ACE", "Trustee.Domain", "Trustee.Name" };
            string[] filter = new string[] { "Trustee.Domain=mss", "Trustee.Name=lcosta" };
            
            Dictionary<string, object> result = this.InvokeMethod(parentObjects, propertyName, filter);
            
            Assert.IsNotNull(result, "The result of Wmi Invoke Method cannot be null");
            Assert.AreEqual(4, result.Count, "The result count is invalid.");
        }
         
        [TestMethod, Owner("lfernandes")]
        public void LearningTest_StringSplitMethod_that_should_return_own_string_when_the_separator_is_not_present_in_it()
        {
            string stringToSplit = "lfernandes";
            string[] separator = new string[] { "." };


            string[] splitResult = stringToSplit.Split(separator, StringSplitOptions.None);


            Assert.IsNotNull(splitResult, "The split result cannot be null");
            Assert.AreEqual(1, splitResult.Count(), "The result of split function must contains just one element.");
            Assert.AreEqual(stringToSplit, splitResult[0], "The result should be the own string");
        }

        [TestMethod, Owner("lfernandes")]
        public void LearningTest_Testing_how_DateTime_converts_DateTimeValue_To_StringFormats()
        {
            var now = DateTime.Now;

            string msg = string.Format("[ToLongDateString] = {0}\r\n", now.ToLongDateString());
            msg += string.Format("[ToLongTimeString] = {0}\r\n", now.ToLongTimeString());
            msg += string.Format("[ToLocalTime] = {0}\r\n", now.ToLocalTime());
            msg += string.Format("[ToShortDateString] = {0}\r\n", now.ToShortDateString());
            msg += string.Format("[ToShortTimeString] = {0}\r\n", now.ToShortTimeString());

            var integerValue = Convert.ToInt32(5.41999999f);
            Assert.AreEqual(5, integerValue);
        }

        

        private Dictionary<string, object> InvokeMethod(string[] parentObjects, string[] propertyNames, string[] filterItems)
        {
            MyObject rootObject = (MyObject)this.getDescriptor();
            string fullObjectParentName = parentObjects.First();


            MyObject currentObject = (MyObject)rootObject.Properties[parentObjects.First()];
            for (int i = 1; i < parentObjects.Count(); i++)
            {
                string currentObjectName = parentObjects[i];
                object currentProperty = currentObject.Properties[currentObjectName];
                bool IsCurrObjList = (currentProperty is IEnumerable<MyObject>);
                
                currentObject = IsCurrObjList ? this.getNewCurrentObjByFilter(currentProperty, filterItems) : (MyObject)currentProperty;
                
                fullObjectParentName = fullObjectParentName + "." + currentObjectName;
            }

            Dictionary<string, object> invokeMethodResult = new Dictionary<string, object>();
            foreach (var propertyName in propertyNames)
            {
                string newResultItemKey = string.Format("{0}.{1}", fullObjectParentName, propertyName);
                object newResultItemValue = this.getInvokeResultItemValue(currentObject, propertyName);

                invokeMethodResult.Add(newResultItemKey, newResultItemValue);
            }


            return invokeMethodResult;
        }

        
        
        private object getDescriptor()
        {
            MyObject trusteeLCOSTA = this.getTrustee("123", "mss", "lcosta");
            MyObject trusteeMGASPAR = this.getTrustee("456", "mss", "mgaspar");
            MyObject trusteeLFERNANDES = this.getTrustee("666", "mss", "lfernandes");


            MyObject[] DACLs = new MyObject[] { 
                this.createDACL(trusteeLCOSTA, "XXX", "000", Guid.NewGuid().ToString()),
                this.createDACL(trusteeMGASPAR, "YYY", "101", Guid.NewGuid().ToString()),
                this.createDACL(trusteeLFERNANDES, "ZZZ", "010", Guid.NewGuid().ToString())
            };

            MyObject descriptor = new MyObject();
            descriptor.AddProperty("DACL", DACLs);

            MyObject WmiResult = new MyObject();
            WmiResult.AddProperty("Descriptor", descriptor);

            return WmiResult;
        }

        private MyObject getTrustee(string SID, string domain, string name)
        {
            MyObject trusteeObj = new MyObject();
            trusteeObj.AddProperty("SID", SID);
            trusteeObj.AddProperty("Domain", domain);
            trusteeObj.AddProperty("Name", name);

            return trusteeObj;
        }

        private MyObject createDACL(MyObject trustee, string mask, string ace, string token)
        {
            MyObject newDACL = new MyObject();
            newDACL.AddProperty("Trustee", trustee);
            newDACL.AddProperty("AccessMask", mask);
            newDACL.AddProperty("AccessToken", token);
            newDACL.AddProperty("ACE", ace);

            return newDACL;
        }



        private MyObject getNewCurrentObjByFilter(object currentObject, string[] filterItems)
        {
            MyObject[] filterResult = this.ApplyFilter(currentObject, filterItems.First());
            for (int j = 1; j < filterItems.Count(); j++)
                filterResult = this.ApplyFilter(filterResult, filterItems[j]);

            return (MyObject)filterResult.SingleOrDefault();
        }

        private MyObject[] ApplyFilter(object allObjectsToFilter, string filterItem)
        {
            KeyValuePair<string, string> filter = this.ConvertStringToKeyValuePair(filterItem, "=");
            string[] filterProperties = filter.Key.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            IList<MyObject> filterResult = new List<MyObject>();
            
            foreach (var obj in (IEnumerable<MyObject>)allObjectsToFilter)
            {
                string parentPropertyName = string.Empty;
                for (int i = 0; i < filterProperties.Count(); i++)
                {
                    
                    if (this.IsLastElementInList(i, filterProperties))
                    {
                        MyObject currentProperty = (MyObject)obj.Properties[parentPropertyName];
                        string propertyValue = currentProperty.Properties[filterProperties[i]].ToString();
                        this.UpdateFilter(propertyValue, filter.Value, obj, filterResult);
                    }
                    else
                        parentPropertyName = filterProperties[i];
                }
            }

            return filterResult.ToArray();
        }

        private KeyValuePair<string, string> ConvertStringToKeyValuePair(string value, string separator)
        {
            string[] splittedString = value.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            string pairKey = splittedString.First();
            string pairValue = (splittedString.Count() > 1) ? splittedString.Last() : null;
            
            return new KeyValuePair<string, string>(pairKey, pairValue);
        }

        private bool IsLastElementInList(int elementIndex, object[] list)
        {
            int nonZeroBasedIndex = elementIndex + 1;
            return (nonZeroBasedIndex == list.Count());
        }

        private void UpdateFilter(string propertyValue, string filterValue, MyObject objectToAdd, IList<MyObject> filterResult)
        {
            if (propertyValue.Equals(filterValue, StringComparison.CurrentCultureIgnoreCase))
                filterResult.Add(objectToAdd);
        }

        private object getInvokeResultItemValue(object objectResult, string propertyName)
        {
            KeyValuePair<string, string> property = this.ConvertStringToKeyValuePair(propertyName, ".");

            object objectProperty = ((MyObject)objectResult).Properties[property.Key];
            bool isSimpleProperty = string.IsNullOrEmpty(property.Value);

            return isSimpleProperty ? objectProperty : ((MyObject)objectProperty).Properties[property.Value];
        }



        private ItemType getFakeFileItem()
        {
            return new fileeffectiverights_item()
            {
                trustee_name = new EntityItemStringType() { Value = "mss\\lfernandes" },
                path = new EntityItemStringType() { Value = "c:\temp" },
                filename = new EntityItemStringType() { Value = "file1.txt" }
            };
        }
    }
}
