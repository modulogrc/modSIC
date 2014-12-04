/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using Microsoft.Win32;
using Modulo.Collect.OVAL.SystemCharacteristics;
using System.Text;

namespace Modulo.Collect.Probe.Windows.Registry
{
    public enum eHiveNames: uint
    {
        HKEY_CLASSES_ROOT = 0x80000000,
        HKEY_CURRENT_USER = 0x80000001,
        HKEY_LOCAL_MACHINE = 0x80000002,
        HKEY_USERS = 0x80000003,
        HKEY_CURRENT_CONFIG = 0x80000005
    }

    public enum eValueTypes: int
    {
        NOT_FOUND = -1,
        STRING = 1,
        EXPANDED_STRING = 2,
        BINARY = 3,
        DWORD = 4,
        DWORD_LITTLE_ENDIAN = 5,
        MULTI_STRING = 7
    }

    public class RegistryHelper
    {
        public static String ConvertToGetValueMethodNameFromValueType(eValueTypes valueType)
        {
            switch (valueType)
            {
                case eValueTypes.BINARY:
                    return "GetBinaryValue";
                case eValueTypes.DWORD:
                case eValueTypes.DWORD_LITTLE_ENDIAN:
                    return "GetDWORDValue";
                case eValueTypes.EXPANDED_STRING:
                    return "GetExpandedStringValue";
                case eValueTypes.MULTI_STRING:
                    return "GetMultiStringValue";
                case eValueTypes.STRING:
                    return "GetStringValue";
                default:
                    throw new Exception("Invalid type while trying to convert valueToMatch type into get valueToMatch method name.");
            }
        }

        public static eHiveNames GetHiveKeyIdFromHiveName(string hiveName)
        {
            return (eHiveNames)Enum.Parse(typeof(eHiveNames), hiveName);
        }

        public static RegistryHive GetRegistryHiveFromHiveName(string hiveName)
        {
            var hiveIDInHiveNamesEnum = (eHiveNames)Enum.Parse(typeof(eHiveNames), hiveName);
            switch (hiveIDInHiveNamesEnum)
            {
                case eHiveNames.HKEY_CLASSES_ROOT:
                    return RegistryHive.ClassesRoot;
                case eHiveNames.HKEY_CURRENT_USER:
                    return RegistryHive.CurrentUser;
                case eHiveNames.HKEY_LOCAL_MACHINE:
                    return RegistryHive.LocalMachine;
                case eHiveNames.HKEY_USERS:
                    return RegistryHive.Users;
                case eHiveNames.HKEY_CURRENT_CONFIG:
                    return RegistryHive.CurrentConfig;
            }

            throw new Exception("[RegKeyEffectiveRightsOperationEvaluator]: Invalid hive name.");
        }

        public static String GetHiveNameFromHiveKeyID(eHiveNames keyID)
        {
            return Enum.GetName(typeof(eHiveNames), keyID);
        }

        public static String GetEnumValueFromHiveName(string hiveName)
        {
            var value = (uint)Enum.Parse(typeof(eHiveNames), hiveName);
            return value.ToString();
        }

        public static String GetValueTypeAsString(eValueTypes valueTypeID)
        {
            return Enum.GetName(typeof(RegistryItemTypeType), valueTypeID);
        }

        public static Dictionary<String, String> GetInputParametersForWmiEnumValuesMethod(string hiveName, string parentKeyName)
        {
            var inputParametersEnumValuesMethod = new Dictionary<String, String>();
            inputParametersEnumValuesMethod.Add("hDefKey", RegistryHelper.GetEnumValueFromHiveName(hiveName));
            inputParametersEnumValuesMethod.Add("sSubKeyName", parentKeyName);
            
            return inputParametersEnumValuesMethod;
        }

        public static String RegistryBinaryToString(byte[] registryBinaryValue)
        {
            var stringBuilder = new StringBuilder();
            foreach (var oneByte in registryBinaryValue)
                stringBuilder.AppendFormat("{0:x2}", oneByte);

            return stringBuilder.ToString();
        }
    }
}
