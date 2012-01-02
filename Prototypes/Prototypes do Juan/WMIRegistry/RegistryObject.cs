using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FrameworkNG.WMI.Registry
{
    public abstract class RegistryObject
    {
        #region "fields"
        private bool isConnected;
        private string currentSize; //Win32_Registry properties placeholders
        private string caption;
        private string description;
        private string installDate;
        private string maximumSize;
        private string name;
        private string proposedSize;
        private string status;
        #endregion
   
        #region "properties"
        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; }
        }
        public string CurrentSize
        {
            get { return currentSize; }
            set { currentSize = value; }
        }
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string InstallDate
        {
            get { return installDate; }
            set { installDate = value; }
        }
        public string MaximumSize
        {
            get { return maximumSize; }
            set { maximumSize = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string ProposedSize
        {
            get { return proposedSize; }
            set { proposedSize = value; }
        }
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        #endregion

        public abstract void CreateKey(baseKey RootKey, string key);
        public abstract void DeleteKey(baseKey RootKey, string key);
        public abstract bool KeyExists(baseKey RootKey, string key);
        public abstract ArrayList EnumerateKeys(baseKey RootKey, string key);
        public abstract ArrayList SearchKeys(baseKey RootKey, string keypattern);
        public abstract ArrayList SearchKeys(baseKey RootKey, string keypattern, string vnamepattern);
        public abstract ArrayList EnumerateValues(baseKey RootKey, string key);
        public abstract ArrayList EnumerateValuesWithTypes(baseKey RootKey, string key, out int[] types);
        public abstract string GetValue(baseKey RootKey, string key, string valueName, valueType ValueType);
        public abstract void SetValue(baseKey RootKey, string key, string valueName, string value, valueType ValueType);
        public abstract void CreateValue(baseKey RootKey, string key, string valueName, string value);
        public abstract void DeleteValue(baseKey RootKey, string key, string valueName);
        public abstract void GetRegistryProperties();
        public abstract bool Connect();
       
    }
}
