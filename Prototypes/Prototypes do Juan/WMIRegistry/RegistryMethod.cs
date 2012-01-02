using System;
using System.Collections;
using System.Management;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FrameworkNG.WMI.Registry
{
    public class RegistryMethod
    {
        public static Boolean IsPattern(string target)
        {
            target = target.Replace(' ', '_');
            string escapedtarget = Regex.Escape(target);
            return (escapedtarget != target);
        }
        public static void CreateKey(ManagementScope connectionScope,
                                     baseKey BaseKey,
                                     string key)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("CreateKey");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("CreateKey",
                                                                        methodParams, null);
        }
        public static void DeleteKey(ManagementScope connectionScope,
                                     baseKey BaseKey,
                                     string key)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("DeleteKey");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("DeleteKey",
                                                                        methodParams, null);
        }
        public static void CreateValue(ManagementScope connectionScope,
                                       baseKey BaseKey,
                                       string key,
                                       string valueName,
                                       string value)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("SetStringValue");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;
            methodParams["sValue"] = value;
            methodParams["sValueName"] = valueName;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("SetStringValue",
                                                                        methodParams, null);
        }
        public static void DeleteValue(ManagementScope connectionScope,
                                       baseKey BaseKey,
                                       string key,
                                       string valueName)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("DeleteValue");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;
            methodParams["sValueName"] = valueName;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("DeleteValue",
                                                                        methodParams, null);
        }
        public static ArrayList SearchKeys(ManagementScope connectionScope,
                                             baseKey BaseKey,
                                             string keypattern,
                                             string vnamepattern)
        {
            string[] components = keypattern.Split('\\');
            return SearchKeys(connectionScope, BaseKey, components, 0, vnamepattern);
        }
        public static ArrayList SearchKeys(ManagementScope connectionScope,
                                             baseKey BaseKey,
                                             string keypattern)
        {
            string[] components = keypattern.Split('\\');
            return SearchKeys(connectionScope, BaseKey, components, 0, null);
        }
        public static ArrayList SearchKeys(ManagementScope connectionScope,
                                             baseKey BaseKey,
                                             string[] components,
                                             int startlevel,
                                             string vnamepattern)
        {
            ArrayList retVal = new ArrayList();
            string constantpart = "";
            string onekey;

            for (int i = 0; i < startlevel; i++)
            {
                if (i == 0)
                    constantpart = components[i];
                else
                    constantpart += "\\" + components[i];
            }

            for (int i = startlevel; i <= components.GetUpperBound(0); i++)
            {
                if (IsPattern(components[i]))
                {
                    string myPattern = components[i];
                    string[] thislevelkeys = EnumerateKeys(connectionScope, BaseKey, constantpart);
                    if (thislevelkeys == null)
                        return retVal;
                    if (i == components.GetUpperBound(0))
                    {
                        foreach (string altpath in thislevelkeys)
                        {
                            if (Regex.Match(altpath, myPattern).Success)
                            {
                                components[i] = altpath;
                                onekey = String.Join("\\", components);
                                if (vnamepattern == null)
                                    retVal.Add(onekey);
                                else
                                {
                                    ArrayList matchingvals = SearchValues(connectionScope, BaseKey, onekey, vnamepattern);
                                    if (matchingvals != null)
                                    {
                                        foreach (string thisval in matchingvals)
                                        {
                                            retVal.Add(new RegKeyValue(onekey, thisval));
                                        }
                                    }
                                }
                            }
                        }
                        return retVal;
                    }
                    else
                    {
                        foreach (string altpath in thislevelkeys)
                        {
                            if (Regex.Match(altpath, myPattern).Success)
                            {
                                string[] bkpcomp = (string[]) components.Clone();
                                components[i] = altpath;
                                retVal.AddRange(SearchKeys(connectionScope, BaseKey, components, i + 1, vnamepattern));
                                for (int j = i; j <= components.GetUpperBound(0); j++)
                                {
                                    components[j] = bkpcomp[j];
                                }
                            }
                        }
                        return retVal;
                    }
                }
                else
                {
                    if (constantpart == "")
                        constantpart = components[i];
                    else
                        constantpart += "\\" + components[i];
                }
            }
            onekey = String.Join("\\", components);
            if (KeyExists(connectionScope, BaseKey, onekey))
            {
                if (vnamepattern == null)
                    retVal.Add(onekey);
                else
                {
                    ArrayList matchingvals = SearchValues(connectionScope, BaseKey, onekey, vnamepattern);
                    if (matchingvals != null)
                    {
                        foreach (string thisval in matchingvals)
                        {
                            retVal.Add(new RegKeyValue(onekey, thisval));
                        }
                    }
                }
            }
            return retVal;
        }
        public static ArrayList SearchValues(ManagementScope connectionScope,
                                                 baseKey BaseKey,
                                                 string key,
                                                 string vnamepattern)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("EnumValues");
            ArrayList retVal = new ArrayList();

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("EnumValues",
                                                        methodParams, null);
            System.String[] values;
            values = (string[])exitCode["sNames"];
            if (values != null)
            {
                bool addthis;

                foreach (string value in values)
                {
                    if (IsPattern(vnamepattern))
                        addthis = Regex.Match(value, vnamepattern).Success;
                    else
                        addthis = vnamepattern.Equals(value, StringComparison.CurrentCultureIgnoreCase);

                    if (addthis)
                        retVal.Add(value);
                }
            }
            return retVal;
        }
        public static bool KeyExists(ManagementScope connectionScope,
                                     baseKey BaseKey,
                                     string key)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("CheckAccess");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;
            methodParams["uRequired"] = 1;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("CheckAccess",
                                                        methodParams, null);

            object outbGranted = exitCode["bGranted"];
            object outReturnValue = exitCode["ReturnValue"];

            if (outReturnValue.ToString() == "0")
                return true;
            else
                return false;
        }
        public static string[] EnumerateKeys(ManagementScope connectionScope,
                                             baseKey BaseKey,
                                             string key)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("EnumKey");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("EnumKey",
                                                        methodParams, null);
            System.String[] subKeys;
            subKeys = (string[])exitCode["sNames"];
            return subKeys;
        }
        public static string[] EnumerateValues(ManagementScope connectionScope,
                                               baseKey BaseKey,
                                               string key)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("EnumValues");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("EnumValues",
                                                        methodParams, null);
            System.String[] values;
            values = (string[])exitCode["sNames"];
            return values;
        }
        public static string[] EnumerateValuesWithTypes(ManagementScope connectionScope,
                                               baseKey BaseKey,
                                               string key,
                                               out int[] types)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("EnumValues");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("EnumValues",
                                                        methodParams, null);
            System.String[] values;
            types = (int[])exitCode["Types"];
            values = (string[])exitCode["sNames"];
            return values;
        }
        public static string GetValue(ManagementScope connectionScope,
                                      baseKey BaseKey,
                                      string key,
                                      string valueName,
                                      valueType ValueType)
        {
            string typeOfValue = RegistryMethod.ConvertGetValueType(ValueType);
            string returnValue = string.Empty;
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters(typeOfValue);

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;
            methodParams["sValueName"] = valueName;

            ManagementBaseObject exitValue = registryTask.InvokeMethod(typeOfValue,
                                                                     methodParams, null);
            try{
                returnValue = exitValue["sValue"].ToString();
            }
            catch{
                try{ //ToDo: fix this ASAP, nested try/catch, I mean come on dude!
                    returnValue = exitValue["uValue"].ToString();
                }
                catch (SystemException e){
                    returnValue = e.Message.ToString();
                }
            }
            return returnValue;
        }
        public static void SetValue(ManagementScope connectionScope,
                                     baseKey BaseKey,
                                     string key,
                                     string valueName,
                                     string value,
                                     valueType ValueType)
        {
            string typeOfValue = RegistryMethod.ConvertSetValueType(ValueType);
            string returnValue = string.Empty;
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters(typeOfValue);

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;
            methodParams["sValueName"] = valueName;
            methodParams["sValue"] = value;

            ManagementBaseObject exitValue = registryTask.InvokeMethod(typeOfValue,
                                                                     methodParams, null);
        }

        #region "helpers"
        public static string ConvertGetValueType(valueType entry)
        {
            string translation = string.Empty;
            switch (entry)
            {
                case valueType.BINARY:
                    translation = "GetBinaryValue";
                    break;
                case valueType.DWORD:
                    translation = "GetDWORDValue";
                    break;
                case valueType.EXPANDED_STRING:
                    translation = "GetExpandedStringValue";
                    break;
                case valueType.MULTI_STRING:
                    translation = "GetMultiStringValue";
                    break;
                case valueType.STRING:
                    translation = "GetStringValue";
                    break;
            }
            return translation;
        }
        public static string ConvertSetValueType(valueType entry)
        {
            string translation = string.Empty;
            switch (entry)
            {
                case valueType.BINARY:
                    translation = "SetBinaryValue";
                    break;
                case valueType.DWORD:
                    translation = "SetDWORDValue";
                    break;
                case valueType.EXPANDED_STRING:
                    translation = "SetExpandedStringValue";
                    break;
                case valueType.MULTI_STRING:
                    translation = "SetMultiStringValue";
                    break;
                case valueType.STRING:
                    translation = "SetStringValue";
                    break;
            }
            return translation;
        }
        #endregion
       
    }
}
