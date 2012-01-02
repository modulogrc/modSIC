using System;
using System.Collections;
using System.Management;
using System.Collections.Generic;
using System.Text;

namespace FrameworkNG.WMI.Registry
{
    public class RegistryRemote:RegistryObject 
    {
        #region "fields"
        private string userName; //object placeholders
        private string password;
        private string domain;
        private string machineName;
        private ManagementScope connectionScope;
        private ConnectionOptions options;
       
        #endregion

        #region "constructors"
        public RegistryRemote(string userName,
                             string password,
                             string domain,
                             string machineName)
        {
            this.userName = userName;
            this.password = password;
            this.domain = domain;
            this.machineName = machineName;
            options = RegistryConnection.RegistryConnectionOptions();

            Connect();
            GetRegistryProperties();
        }
        #endregion
       
        #region "polymorphic methods"
        public override void CreateKey(baseKey RootKey, string key)
        {
            if (this.IsConnected)
                try
                {
                    RegistryMethod.CreateKey(connectionScope, RootKey, key);
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
        }
        public override void DeleteKey(baseKey RootKey, string key)
        {
            if (IsConnected)
                try
                {
                    RegistryMethod.DeleteKey(connectionScope, RootKey, key);
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
        }
        public override ArrayList SearchKeys(baseKey RootKey, string keypattern)
        {
            ArrayList subKeys = new ArrayList();
            if (IsConnected)
                try
                {
                    subKeys.AddRange(RegistryMethod.SearchKeys(connectionScope, RootKey, keypattern));
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
            return subKeys;
        }
        public override ArrayList SearchKeys(baseKey RootKey, string keypattern, string vnamepattern)
        {
            ArrayList subKeys = new ArrayList();
            if (IsConnected)
                try
                {
                    subKeys.AddRange(RegistryMethod.SearchKeys(connectionScope, RootKey, keypattern, vnamepattern));
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
            return subKeys;
        }
        public override bool KeyExists(baseKey RootKey, string key)
        {
            bool isThere = false;
            if (IsConnected)
                try
                {
                    isThere = RegistryMethod.KeyExists(connectionScope, RootKey, key);
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
            return isThere;
        }
        public override ArrayList EnumerateKeys(baseKey RootKey, string key)
        {
            ArrayList subKeys = new ArrayList();
            if (IsConnected)
                try
                {
                    subKeys.AddRange(RegistryMethod.EnumerateKeys(connectionScope, RootKey, key));
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
            return subKeys;
        }
        public override ArrayList EnumerateValues(baseKey RootKey, string key)
        {
            ArrayList alValues = new ArrayList();
            if (IsConnected)
                try
                {
                    alValues.AddRange(RegistryMethod.EnumerateValues(connectionScope, RootKey, key));
                    alValues.Sort();
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
            return alValues;
        }
        public override ArrayList EnumerateValuesWithTypes(baseKey RootKey, string key, out int[] types)
        {
            ArrayList alValues = new ArrayList();
            int[] retTypes = null;
            if (IsConnected)
                try
                {
                    alValues.AddRange(RegistryMethod.EnumerateValuesWithTypes(connectionScope, RootKey, key, out retTypes));
                    alValues.Sort();
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
            types = retTypes;
            return alValues;
        }
        public override string GetValue(baseKey RootKey, string key, string valueName, valueType ValueType)
        {
            string Value = string.Empty;
            if (IsConnected)
                try
                {
                    Value = RegistryMethod.GetValue(connectionScope, RootKey, key, valueName, ValueType);
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
            return Value;
        }
        public override void SetValue(baseKey RootKey, string key, string valueName, string value, valueType ValueType)
        {
            if (IsConnected)
                try
                {
                    RegistryMethod.SetValue(connectionScope, RootKey, key, valueName, value, ValueType);
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
        }
        public override void GetRegistryProperties()
        {
            if (IsConnected)
            {
                RegistryConnection.GetRegistryProperties(machineName, options, this);
            }
        }
        public override void CreateValue(baseKey RootKey, string key, string valueName, string value)
        {
            if (IsConnected)
                try
                {
                    RegistryMethod.CreateValue(connectionScope, RootKey, key, valueName, value);
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
        }
        public override void DeleteValue(baseKey RootKey, string key, string valueName)
        {
            if (IsConnected)
                try
                {
                    RegistryMethod.DeleteValue(connectionScope, RootKey, key, valueName);
                }
                catch (ManagementException err)
                {
                    Console.WriteLine("An error occurred: " + err.Message);
                }
        }
        public override bool Connect()
        {
            if (domain != null || userName != null)
            {
                options.Username = domain + "\\" + userName;
                options.Password = password;
            }
            connectionScope = RegistryConnection.ConnectionScope(machineName, options, this);
            return this.IsConnected;
        }
        #endregion     
    }
}
