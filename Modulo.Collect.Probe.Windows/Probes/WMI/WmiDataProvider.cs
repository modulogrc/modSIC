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
using System.Management;
using System.Text;


namespace Modulo.Collect.Probe.Windows.WMI
{
    public class WmiDataProvider
    {
        private ManagementScope _connectionScope;
        public ManagementScope ConnectionScope { get { return this._connectionScope; } }

        /// <summary>
        /// Class that provider access to Remote Wmi Classes.
        /// </summary>
        /// <param name="connectionScope">The management scope. 
        /// It must contain the correct namespace for wmi class access and a valid connection state.</param>
        public WmiDataProvider(ManagementScope connectionScope)
        {
            this._connectionScope = connectionScope;
        }

        /// <summary>
        /// Class that provider access to Remote Wmi Classes.
        /// </summary>
        /// <param name="wmiNamespace">Namespace of wmi class for ManagementScope creation</param>
        public WmiDataProvider(string wmiNamespace)
        {
            this._connectionScope = new ManagementScope(new ManagementPath(wmiNamespace));
            this._connectionScope.Connect();
        }

        /// <summary>
        /// Class that provider access to Remote Wmi Classes.
        /// </summary>
        /// <param name="connectionScope">The management scope. 
        /// It must contain the correct namespace for wmi class access and a valid connection state.</param>
        public WmiDataProvider() { }

        /// <summary>
        /// It invokes a wmi method and returns all output parameters with your values.
        /// The management scope given on construction, must contains the correct wmi class to execute this method. 
        /// </summary>
        /// <param name="methodName">The name of method</param>
        /// <param name="inParameters">The KeyValuePair list of in parameters. The Key is the name of in parameter while the Value gives the valueToMatch of in parameter.</param>
        /// <returns>a KeyValuePair List of out Parameters. The Key is the name of out parameter while the Value gives the valueToMatch of out parameter.</returns>
        public virtual Dictionary<String, Object> InvokeMethod(String methodName, Dictionary<String, string> inParameters)
        {
            ManagementBaseObject wmiMethodResult = this.getWMIMethodHandler(methodName, inParameters);

            Dictionary<String, Object> outParameters = new Dictionary<String, Object>();
            foreach (var outParameter in wmiMethodResult.Properties)
                outParameters.Add(outParameter.Name, outParameter.Value);

            return outParameters;
        }

        /// <summary>
        /// It invokes a wmi method and returns a single result.
        /// The management scope given on construction, must contains the correct wmi class to execute this method. 
        /// </summary>
        /// <param name="methodName">The name of method</param>
        /// <param name="inParameters">The KeyValuePair list of in parameters. The Key is the name of in parameter while the Value gives the valueToMatch of in parameter.</param>
        /// <param name="outParameterName">The name of output parameter which contains the data that will be collected.</param>
        /// <returns>A object (or a list) that holds the collected data of given output parameter.</returns>
        public virtual Object InvokeMethod(String methodName, Dictionary<String, string> inParameters, String outParameterName)
        {
            Dictionary<String, Object> outParameters = this.InvokeMethod(methodName, inParameters);
            return outParameters[outParameterName];
        }

        /// <summary>
        /// Gets the wmi object properties via WQL.
        /// </summary>
        /// <param name="wmiClassName">The name of WMI Class that represents the WMI Object.</param>
        /// <param name="inParameters">List of parameters to build WQL filter.</param>
        /// <returns>Returns a dictionary that contains the wmi object properties and values in Key/Value pair format.</returns>
        public virtual Dictionary<String, Object> GetWmiObjectProperties(String wmiClassName, Dictionary<String, String> inParameters)
        {
            IEnumerable<WmiObject> wmiResults = this.SearchWmiObjects(wmiClassName, inParameters);

            Dictionary<string, object> wmiObjectProperties = new Dictionary<string, object>();
            foreach (var wmiObject in wmiResults)
            {
                foreach (var wmiObjects in wmiObject.GetValues())
                {
                    wmiObjectProperties.Add(wmiObjects.Key, wmiObjects.Value);

                }
            }

            return wmiObjectProperties;
        }

        /// <summary>
        /// Gets wmi objects via WMI.
        /// </summary>
        /// <param name="wmiClassName">The WMI Class that represents WMI Object.</param>
        /// <param name="inParameters">List of parameters to build WQL filter.</param>
        /// <param name="propertyName">The name of object property</param>
        /// <returns>Returns a list that contains a property value of wmi object.</returns>
        public virtual IList<Object> GetWmiObjects(String wmiClassName, Dictionary<String, String> inParameters, string propertyName)
        {
            IEnumerable<WmiObject> wmiResults = this.SearchWmiObjects(wmiClassName, inParameters);

            IList<Object> wmiObjects = new List<Object>();
            foreach (var obj in wmiResults)
            {
                wmiObjects.Add(obj.GetValueOf(propertyName));
            }

            return wmiObjects;
        }

        public virtual IEnumerable<WmiObject> SearchWmiObjects(String wmiClassName, Dictionary<String, String> inParameters)
        {
            string wmiCommandQuery = new WQLBuilder().WithWmiClass(wmiClassName).AddParameters(inParameters).Build();
            return this.executeWQLCommandQuery(wmiCommandQuery);
        }

        public virtual IEnumerable<WmiObject> ExecuteWQL(string wqlCommandQuery)
        {
            return this.executeWQLCommandQuery(wqlCommandQuery);
        }

        public virtual object InvokeMethodByWmiPath(WmiInvokeMethodInfo wmiInvokeMethodInfo)
        {
            ManagementPath mngPath = this.getManagementPath(wmiInvokeMethodInfo.ClassName, wmiInvokeMethodInfo.PathName, wmiInvokeMethodInfo.PathValue);
            ManagementObject mngObject = new ManagementObject(this._connectionScope, mngPath, null);
            object methodReturn = null;
            try
            {
                methodReturn = mngObject.InvokeMethod(wmiInvokeMethodInfo.MethodName, null, null);
                if (methodReturn == null)
                    throw new InvalidInvokeMethodException(mngPath.RelativePath, wmiInvokeMethodInfo.MethodName);
            }
            catch (ManagementException)
            {
                throw new InvalidInvokeMethodException(mngPath.RelativePath, wmiInvokeMethodInfo.MethodName);
            }



            if (wmiInvokeMethodInfo.ShouldBeReturnManagementObjects())
                return methodReturn;


            ManagementBaseObject systemObject = (ManagementBaseObject)methodReturn;
            string fullObjectParentName = wmiInvokeMethodInfo.OutParameters.ParentObjectNames.First();
            ManagementBaseObject currentObject = (ManagementBaseObject)systemObject.Properties[fullObjectParentName].Value;

            for (int i = 1; i < wmiInvokeMethodInfo.OutParameters.ParentObjectNames.Count; i++)
            {
                string currentObjectName = wmiInvokeMethodInfo.OutParameters.ParentObjectNames.ElementAt(i);
                object currentProperty = currentObject.Properties[currentObjectName].Value;
                bool IsCurrObjList = (currentProperty is IEnumerable<ManagementBaseObject>);

                currentObject = IsCurrObjList ? this.getNewCurrentObjByFilter(currentProperty, wmiInvokeMethodInfo.Filter) : (ManagementBaseObject)currentProperty;
                fullObjectParentName = fullObjectParentName + "." + currentObjectName;
            }

            if (currentObject == null)
                throw new InvalidInvokeMethodFilterException(mngPath.RelativePath, wmiInvokeMethodInfo.MethodName);

            IList<String> propertyNames = wmiInvokeMethodInfo.OutParameters.PropertyNames;
            var newWmiObject = this.createWmiObjectWithProperties(currentObject, fullObjectParentName, propertyNames);
            return new List<WmiObject>(new WmiObject[] { newWmiObject });
        }

        /// <summary>
        /// It tries to establish a connection to Windows system through WMI
        /// </summary>
        /// <param name="targetIpOrHostname">The IP or hostname of target system.</param>
        /// <param name="username">A user with administrative privileges.</param>
        /// <param name="password">User password.</param>
        /// <returns>True if the connection was established, otherwise False</returns>
        public virtual Boolean IsTargetWindowsSystem(string targetAddress, string username, string password)
        {
            var @namespace = String.Format(@"\\{0}\root\cimv2", targetAddress);
            var connectionOptions = WmiDataProviderFactory.CreateConnectionOptions(username, password);
            var connectionScope = new ManagementScope(@namespace, connectionOptions);

            try
            {
                connectionScope.Connect();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        #region Private Methods
        private static string getPropertyNameValue(PropertyData innerProperty)
        {
            string propertyName = null;
            string propertyValue = null;
            try
            {
                propertyName = ((PropertyData)innerProperty).Name;
                propertyValue = ((PropertyData)innerProperty).Value.ToString();
            }
            catch (Exception)
            {
                propertyValue = "[Error]";
            }

            return string.Format("[{0}]={1}", propertyName, propertyValue);
        }

        private ManagementBaseObject getNewCurrentObjByFilter(object currentObject, IEnumerable<WmiInvokeMethodFilter> filterItems)
        {
            ManagementBaseObject[] filterResult = this.ApplyFilter(currentObject, filterItems.First());
            for (int i = 1; i < filterItems.Count(); i++)
                filterResult = this.ApplyFilter(currentObject, filterItems.ElementAt(i));

            return (ManagementBaseObject)filterResult.SingleOrDefault();
        }

        private ManagementBaseObject[] ApplyFilter(object allObjectsToFilter, WmiInvokeMethodFilter filterItem)
        {
            IList<ManagementBaseObject> filterResult = new List<ManagementBaseObject>();
            foreach (var obj in (IEnumerable<ManagementBaseObject>)allObjectsToFilter)
            {
                string parentPropertyName = string.Empty;
                for (int i = 0; i < filterItem.FilterParentObjectNames.Count; i++)
                {
                    parentPropertyName = filterItem.FilterParentObjectNames[i];
                    if (i == filterItem.FilterParentObjectNames.Count - 1)
                    {
                        ManagementBaseObject systemParentObj = (ManagementBaseObject)obj.Properties[parentPropertyName].Value;
                        if (this.IsSystemObjectMatch(systemParentObj, filterItem.FilterPropertyName, filterItem.FilterPropertyValue))
                            filterResult.Add(obj);
                    }
                }
            }

            return filterResult.ToArray();
        }

        private bool IsSystemObjectMatch(ManagementBaseObject systemObject, string propertyName, string propertyValue)
        {
            PropertyData systemObjectProperty = systemObject.Properties[propertyName];
            string systemObjectPropertyValue = systemObjectProperty.Value.ToString();

            return (systemObjectPropertyValue.Equals(propertyValue, StringComparison.CurrentCultureIgnoreCase));
        }

        private object getInvokeResultItemValue(object objectResult, string propertyName)
        {
            KeyValuePair<string, string> property = this.ConvertStringToKeyValuePair(propertyName, ".");

            object objectProperty = ((ManagementBaseObject)objectResult).Properties[property.Key].Value;
            bool isSimpleProperty = string.IsNullOrEmpty(property.Value);

            return isSimpleProperty ? objectProperty : ((ManagementBaseObject)objectProperty).Properties[property.Value].Value;
        }

        private KeyValuePair<string, string> ConvertStringToKeyValuePair(string value, string separator)
        {
            string[] splittedString = value.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            string pairKey = splittedString.First();
            string pairValue = (splittedString.Count() > 1) ? splittedString.Last() : null;

            return new KeyValuePair<string, string>(pairKey, pairValue);
        }

        private IEnumerable<WmiObject> executeWQLCommandQuery(string wqlCommandQuery)
        {
            ManagementObjectSearcher wmiObjectSearcher = this.createWmiObjectSearcher(wqlCommandQuery);
            return this.createWmiObjectsFromObjectSearcher(wmiObjectSearcher);
        }

        private IEnumerable<WmiObject> createWmiObjectsFromObjectSearcher(ManagementObjectSearcher wmiObjectSearcher)
        {
            List<WmiObject> wmiObjects = new List<WmiObject>();
            foreach (ManagementObject managementObject in wmiObjectSearcher.Get())
            {
                WmiObject wmiObject = new WmiObject();
                foreach (PropertyData property in managementObject.Properties)
                {
                    wmiObject.Add(property.Name, managementObject.GetPropertyValue(property.Name));
                }
                wmiObjects.Add(wmiObject);
            }
            return wmiObjects;
        }

        private WmiObject createWmiObjectsFromSingleObject(ManagementObject managementObject)
        {
            WmiObject wmiObject = new WmiObject();
            foreach (PropertyData property in managementObject.Properties)
                wmiObject.Add(property.Name, managementObject.GetPropertyValue(property.Name));

            return wmiObject;
        }

        private WmiObject createWmiObjectWithProperties(
            ManagementBaseObject systemObject, string parentPropertyName, IList<String> outParameters)
        {
            WmiObject wmiObject = new WmiObject();

            foreach (var outParameterName in outParameters)
            {
                string newResultItemKey = string.Format("{0}.{1}", parentPropertyName, outParameterName);
                object newResultItemValue = this.getInvokeResultItemValue(systemObject, outParameterName);

                wmiObject.Add(newResultItemKey, newResultItemValue);
            }

            return wmiObject;
        }

        private string getFullParentObjectName(IList<String> parentObjects)
        {
            string dot = string.Empty;
            string fullParentName = string.Empty;
            foreach (var objName in parentObjects)
            {
                fullParentName += string.Format("{0}{1}", dot, objName);
                dot = ".";
            }
            return fullParentName;
        }

        private Dictionary<string, IList<ManagementBaseObject>> getDeeperManagementObject(ManagementBaseObject rootSystemObject, List<String> parentObjects)
        {
            Dictionary<string, IList<ManagementBaseObject>> wmiResult = new Dictionary<string, IList<ManagementBaseObject>>();

            IList<ManagementBaseObject> processedSystemObjects = this.processSystemObjectProperty(rootSystemObject, parentObjects.First());
            for (int i = 1; i < parentObjects.Count; i++)
            {
                foreach (var processedObj in processedSystemObjects)
                {
                    var newProcessedObjects = this.processSystemObjectProperty(processedObj, parentObjects[i]);
                    wmiResult.Add(parentObjects[i], newProcessedObjects);
                    processedSystemObjects = newProcessedObjects;
                }
            }

            return wmiResult;
        }

        private IList<ManagementBaseObject> processSystemObjectProperty(ManagementBaseObject systemObject, string propertyName)
        {
            PropertyData objectProperty = systemObject.Properties[propertyName];

            List<ManagementBaseObject> result = new List<ManagementBaseObject>();
            if (objectProperty.IsArray)
                result.AddRange((ManagementBaseObject[])objectProperty.Value);
            else
                result.Add((ManagementBaseObject)objectProperty.Value);

            return result;
        }

        private ManagementBaseObject getWMIMethodHandler(String methodName, Dictionary<String, string> methodInParameters)
        {
            ManagementClass wmiClassHandler = new ManagementClass(this._connectionScope, this.ConnectionScope.Path, null);
            ManagementBaseObject wmiMethodParametersHandler =
                this.getPreparedMethodParameters(wmiClassHandler, methodName, methodInParameters);

            return wmiClassHandler.InvokeMethod(methodName, wmiMethodParametersHandler, null);
        }

        private ManagementBaseObject getPreparedMethodParameters(ManagementClass wmiClassHandler, String methodName, Dictionary<String, string> InParameters)
        {
            ManagementBaseObject wmiMethodParameters = wmiClassHandler.GetMethodParameters(methodName);
            foreach (KeyValuePair<String, string> parameter in InParameters)
                wmiMethodParameters[parameter.Key] = parameter.Value.ToString();

            return wmiMethodParameters;
        }

        private ManagementObjectSearcher createWmiObjectSearcher(string wqlCommand)
        {
            ManagementObjectSearcher wmiObjectSearcher = new ManagementObjectSearcher();
            wmiObjectSearcher.Scope = this.ConnectionScope;
            wmiObjectSearcher.Query = new ObjectQuery(wqlCommand);
            wmiObjectSearcher.Options = new EnumerationOptions() { Rewindable = false, ReturnImmediately = true };

            return wmiObjectSearcher;
        }

        private ManagementPath getManagementPath(string className, string pathName, string pathValue)
        {
            string sManagementPath = String.Format("{0}.{1}='{2}'", className, pathName, pathValue);
            return new ManagementPath() { RelativePath = sManagementPath };
        }
        #endregion

    }


    public class InvalidInvokeMethodException : Exception
    {
        private const string EXCEPTION_MSG = "The wmi object cannot be found on '{0}' management path. The method '{1}' does not return any items.";

        public string ManagementPath { get; private set; }
        public string MethodName { get; private set; }

        public InvalidInvokeMethodException(string managementPath, string methodName)
        {
            this.ManagementPath = managementPath;
            this.MethodName = methodName;
        }


        public override string Message { get { return string.Format(EXCEPTION_MSG, this.ManagementPath, this.MethodName); } }
    }

    public class InvalidInvokeMethodFilterException : InvalidInvokeMethodException
    {
        public InvalidInvokeMethodFilterException(string managementPath, string methodName) : base(managementPath, methodName) { }
    }
}
