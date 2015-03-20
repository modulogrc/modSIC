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
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Modulo.Collect.Probe.Common
{

    public enum CredentialParts { Domain, UserName, Password, AdminPassword } 

    public class Credentials : Dictionary<string, object>
    {
       // public Credentials() { } 

        public Credentials(String domain, String username, String password, String adminPassword)
        {
            this.Add(CredentialParts.Domain.ToString(), domain);
            this.Add(CredentialParts.UserName.ToString(), username);
            this.Add(CredentialParts.Password.ToString(), password);
            this.Add(CredentialParts.AdminPassword.ToString(), adminPassword);
        }

        public override string ToString()
        {
            return String.Format("Credentials pack with {0} info elements", this.Count);
        }

        public string GetDomain()
        {
            var domain = this.getValueFromKeySafely(CredentialParts.Domain.ToString());
            return IsTheDomainEmbeddedInUsername() ? GetDomainFromUserName() : domain;
        }

        public string GetUserName()
        {
            var username = this.getValueFromKeySafely(CredentialParts.UserName.ToString());
            return IsTheDomainEmbeddedInUsername() ? this.GetUsernNameFromUserName() : username;
        }

        public string GetPassword()
        {
            return this.getValueFromKeySafely(CredentialParts.Password.ToString());
        }

        public string GetAdminPassword()
        {
            return this.getValueFromKeySafely(CredentialParts.AdminPassword.ToString());
        }

        /// <summary>
        /// Get the username with domain name (FQDN format).
        /// </summary>
        /// <returns>It returns the username on [DOMAIN]\[USERNAME] format.</returns>
        public string GetFullyQualifiedUsername()
        {
            var domain = this.GetDomain();
            var username = this.GetUserName();

            if (string.IsNullOrWhiteSpace(domain))
            {
                if (string.IsNullOrWhiteSpace(username))
                    return string.Empty;

                domain = ".";
            }

            return string.Format(@"{0}\{1}", domain, username);
        }



        private bool IsTheDomainEmbeddedInUsername()
        {
            var domainFromUsername = GetDomainFromUserName();
            return (!string.IsNullOrWhiteSpace(domainFromUsername));
        }

        private string getValueFromKeySafely(string keyName)
        {
            object value;
            this.TryGetValue(keyName, out value);

            return value == null ? string.Empty : value.ToString();
        }

        private string GetDomainFromUserName()
        {
            var username = this.getValueFromKeySafely(CredentialParts.UserName.ToString());
            var domainAndUsername = username.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            bool hasDomain = (domainAndUsername.Count() > 1);
            
            return hasDomain ? domainAndUsername.First() : string.Empty;
        }

        private string GetUsernNameFromUserName()
        {
            var username = this.getValueFromKeySafely(CredentialParts.UserName.ToString());
            var domainAndUsername = username.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            bool hasDomain = (domainAndUsername.Count() > 1);

            return hasDomain ? domainAndUsername.Last() : username;
        }


    }
}
