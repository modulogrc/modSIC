using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG.WMI.Registry
{
    public class RegKeyValue
    {
        public string KeyName { get; set; }
        public string ValueName { get; set; }
        public valueType ValueType { get; set; }

        public RegKeyValue()
        {
        }

        public RegKeyValue(string strKeyName, string strValueName)
        {
            KeyName = strKeyName;
            ValueName = strValueName;
        }

        public RegKeyValue(string strKeyName, string strValueName, valueType initValueType)
        {
            KeyName = strKeyName;
            ValueName = strValueName;
            ValueType = initValueType;
        }
    }
}
