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
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Windows.WMI;
using System.Management;
using Modulo.Collect.Probe.Windows.File.Helpers;

namespace Modulo.Collect.Probe.Windows
{
    public enum WindowsObjectType { FileOrDirectory, RegistryKey };
    
    public enum SecurityDescriptorType { DACL, SACL };


    public class WindowsSecurityDescriptorDisassembler
    {
        private SecurityDescriptorType SecurityDescriptorType;
        // ======================================
        // WINDOWS SECURITY DESCRIPTOR PROPERTIES
        // ======================================
        // DESCRIPTOR
        //  DACL
        //      AccessMask: uint 
        //      AceFlags: uint
        //      AceType: uint
        //      TRUSTEE
        //          Domain: string
        //          Name: string
        //          SID: string
        //  SACL
        //      AccessMask: uint 
        //      AceFlags: uint
        //      AceType: uint
        //      TRUSTEE
        //          Domain: string
        //          Name: string
        //          SID: string
        // =======================================
        // =======================================

        public WindowsSecurityDescriptorDisassembler() { }

        public WindowsSecurityDescriptorDisassembler(SecurityDescriptorType securityDescriptorType)
        {
            this.SecurityDescriptorType = securityDescriptorType;
        }


        /// <summary>
        /// Creates a list of WinACEs objects from security descriptor management object.
        /// </summary>
        /// <param name="rootManagementObject">The result of invoked method which returns the Security Descriptor as ManagementBaseObject.</param>
        /// <param name="trusteeName">The username formatted such as: "[DOMAIN]\[USERNAME]". For local users use the machine name on [DOMAIN]</param>
        /// <returns>Returns a List of WMIWinACE objects.</returns>
        public virtual IEnumerable<WMIWinACE> GetSecurityDescriptorsFromManagementObject(object rootManagementObject, string userTrusteeName, WmiDataProvider wmiProvider)
        {
            ManagementBaseObject[] ACLs = this.getACLFromManagementObject((ManagementBaseObject)rootManagementObject);

            var result = new List<WMIWinACE>();
            foreach (var acl in ACLs)
            {
                var aclTrustee = (ManagementBaseObject)acl.Properties["Trustee"].Value;
                if (this.DoesACLBelongToUser(aclTrustee, userTrusteeName, wmiProvider))
                {
                    WMIWinACE newWinACE = new WMIWinACE();
                    newWinACE.AccessMask = this.getPropertyValueAsUnsiggnedInteger(acl, "AccessMask");
                    newWinACE.AceFlags = this.getPropertyValueAsUnsiggnedInteger(acl, "AceFlags");
                    newWinACE.AceType = this.getPropertyValueAsUnsiggnedInteger(acl, "AceType");
                    newWinACE.Trustee = this.getWinTrusteeFromManagementObject(aclTrustee);
                    newWinACE.CalculateFileAccessRightsFromAccessMask();

                    result.Add(newWinACE);
                }
            }

            return result;
        }


        /// <summary>
        /// Creates a list of WinACEs objects from security descriptor management object.
        /// </summary>
        /// <param name="rootManagementObject">The result of invoked method which returns the Security Descriptor as ManagementBaseObject.</param>
        /// <param name="trusteeName">The username formatted such as: "[DOMAIN]\[USERNAME]". For local users use the machine name on [DOMAIN]</param>
        /// <returns>Returns a List of WMIWinACE objects.</returns>
        public virtual IEnumerable<WMIWinACE> GetAllSecurityDescriptorsFromManagementObject(object rootManagementObject)
        {
            var ACLs = this.getACLFromManagementObject((ManagementBaseObject)rootManagementObject);

            var result = new List<WMIWinACE>();
            foreach (var acl in ACLs)
            {
                var newWinACE = new WMIWinACE()
                {
                    AccessMask = this.getPropertyValueAsUnsiggnedInteger(acl, "AccessMask"),
                    AceFlags = this.getPropertyValueAsUnsiggnedInteger(acl, "AceFlags"),
                    AceType = this.getPropertyValueAsUnsiggnedInteger(acl, "AceType"),
                    Trustee = this.getWinTrusteeFromManagementObject((ManagementBaseObject)acl.Properties["Trustee"].Value)
                };
                newWinACE.CalculateFileAccessRightsFromAccessMask();

                result.Add(newWinACE);
            }

            return result;
        }

        /// <summary>
        /// Converts a Discretionary Access Mask into WMIWinACE struct.
        /// </summary>
        /// <param name="bitwiseAccessMask">The source access mask as unassigned integer;</param>
        /// <returns>It returns a WMIWinACE struct with all object access rights calculated from Access Mask.</returns>
        public WMIWinACE GetSecurityDescriptorFromAccessMask(uint bitwiseAccessMask)
        {
            WMIWinACE result = new WMIWinACE() { AccessMask = bitwiseAccessMask };
            result.CalculateFileAccessRightsFromAccessMask();
            result.CalculateRegistryKeyAccessRightsFromAccessMask();

            return result;
        }




        private bool DoesACLBelongToUser(ManagementBaseObject daclTrustee, string userTrusteeName, WmiDataProvider wmiProvider)
        {
            var winTrustee = this.getWinTrusteeFromManagementObject(daclTrustee);
            if (userTrusteeName.Equals(winTrustee.SIDString))
                return true;

            string username = this.getPropertyValueAsString(daclTrustee, "Name");
            var wql = new WQLBuilder().WithWmiClass("Win32_Account").AddParameter("SID", userTrusteeName).Build();
            var accountName = wmiProvider.ExecuteWQL(wql);

            if ((accountName.Count() > 0) && accountName.First().GetValueOf("Name").ToString().Equals(username, StringComparison.InvariantCultureIgnoreCase))
                return true;
            
            string userDomain = this.getPropertyValueAsString(daclTrustee, "Domain");
            string[] trusteeParts = userTrusteeName.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            bool matchUsername = username.Equals(trusteeParts.Last(), StringComparison.CurrentCultureIgnoreCase);
            bool matchUserDomain = userDomain.Equals(trusteeParts.First(), StringComparison.CurrentCultureIgnoreCase);

            bool isSystemAccount = (userTrusteeName.IndexOf(@"\") < 0);
            return isSystemAccount ? matchUsername : (matchUsername && matchUserDomain);
        }

        private ManagementBaseObject[] getACLFromManagementObject(ManagementBaseObject rootManagementObject)
        {
            var securityDescriptor = (ManagementBaseObject)rootManagementObject.Properties["Descriptor"].Value;
            if (securityDescriptor == null)
                throw new ACLNotFoundException(this.SecurityDescriptorType);

            var aclName = this.SecurityDescriptorType.ToString();

            var acl = (ManagementBaseObject[])securityDescriptor.Properties[aclName].Value;
            if ((acl == null) || (acl.Count() <= 0))
                throw new ACLNotFoundException(this.SecurityDescriptorType);

            return acl;
        }

        private WMIWinTrustee getWinTrusteeFromManagementObject(ManagementBaseObject trustee)
        {
            return new WMIWinTrustee()
            {
                SID = this.getPropertyValueAsByteArray(trustee, "SID"),
                SIDString = this.getPropertyValueAsString(trustee, "SIDString"),
                Domain = this.getPropertyValueAsString(trustee, "Domain"),
                Name = this.getPropertyValueAsString(trustee, "Name")
            };
        }

        private string getPropertyValueAsString(ManagementBaseObject managementObject, string propertyName)
        {
            PropertyData property = managementObject.Properties[propertyName];
            bool hasValue = ((property != null) && (property.Value != null));

            return hasValue ? property.Value.ToString() : string.Empty;
        }

        private uint getPropertyValueAsUnsiggnedInteger(ManagementBaseObject managementObject, string propertyName)
        {
            PropertyData property = managementObject.Properties[propertyName];
            bool hasValue = ((property != null) && (property.Value != null));

            return hasValue ? (uint)property.Value : 0;
        }

        private byte[] getPropertyValueAsByteArray(ManagementBaseObject managementObject, string propertyName)
        {
            PropertyData property = managementObject.Properties[propertyName];
            bool hasValue = ((property != null) && (property.Value != null));

            return hasValue ? (byte[])property.Value : new byte[] { };
        }

    }

    public class ACLNotFoundException: Exception
    {
        private const string EXCEPTION_MESSAGE = "[Win ACL Disassembler] - here is no {0} for this management object.";

        public ACLNotFoundException(SecurityDescriptorType securityDescriptorType)
            : base(string.Format(EXCEPTION_MESSAGE, securityDescriptorType.ToString())) { }
    }
}
